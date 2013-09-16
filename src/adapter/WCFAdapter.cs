using System.ServiceModel.Channels;
using communication.dds.api.adapter;
using communication.dds.api.adapter.callback;
using communication.dds.api.adapter.publisher;
using communication.dds.api.adapter.qos;
using communication.dds.api.adapter.query;
using System;
using System.Collections.Generic;
using wcf.communication.adapter.wcf.publisher;
using wcf.communication.adapter.wcf.subscriber;

namespace wcf.communication.adapter
{
    public class WCFAdapter<TDataImage> : Adapter<TDataImage>
    {
        private readonly Binding binding;
        private readonly string urlAddress;
        private readonly string topicName;
        private readonly object gate = new object();

        private readonly List<Subscriber<TDataImage>> subscribers = new List<Subscriber<TDataImage>>();

        public WCFAdapter(Binding binding, string urlAddress, string topicName)
        {
            if (binding == null)
            {
                throw new ArgumentNullException("binding");
            }

            if (urlAddress == null)
            {
                throw new ArgumentNullException("urlAddress");
            }

            if (topicName == null)
            {
                throw new ArgumentNullException("topicName");
            }

            this.binding = binding;
            this.urlAddress = urlAddress;
            this.topicName = topicName;
        }

        public override void Dispose()
        {
            lock (gate)
            {
                foreach (var subscriber in subscribers.ToArray())
                {
                    try
                    {
                        subscriber.Dispose();
                        subscribers.Remove(subscriber);
                    }
                    catch
                    {
                    }
                }
            }
        }

        public override bool Subscribe(AdapterReaderQos qos, QueryParameters queryParameters, AdapterDataCallback<TDataImage> clientCallback)
        {
            try
            {
                var subscriber = new Subscriber<TDataImage>(binding, urlAddress, queryParameters, clientCallback);
                lock (gate)
                {
                    subscribers.Add(subscriber);
                }

                subscriber.Subscribe(queryParameters.TopicName);
                return true;
            }
            catch (Exception exception)
            {
                clientCallback.OnError(exception);
            }

            return false;
        }

        public override IEnumerable<TDataImage> Snapshot(AdapterReaderQos qos, QueryParameters queryParameters)
        {
            throw new NotSupportedException("Snapshot operation is not currently supported for WCF implementation!");
        }

        public override DataPublisher<TDataImage> CreatePublisher(AdapterWriterQos qos, string partitionName)
        {
            return new Publisher<TDataImage>(binding, urlAddress, topicName, partitionName);
        }
    }
}