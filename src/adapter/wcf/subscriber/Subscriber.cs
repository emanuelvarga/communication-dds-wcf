using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using communication.dds.api.adapter.callback;
using communication.dds.api.adapter.query;
using System.Diagnostics;

namespace wcf.communication.adapter.wcf.subscriber
{
    public class Subscriber<TDataImage> : AInternalAdapter<TDataImage>, ISubscriber
    {
        private ChannelFactory<ISubscriber> specificChannel;
        private ISubscriber clientProxy;
        private Binding binding;
        private string urlAddress;
        private QueryParameters queryParameters;
        private AdapterDataCallback<TDataImage> clientCallback;

        public Subscriber(Binding binding, string urlAddress, QueryParameters queryParameters, AdapterDataCallback<TDataImage> clientCallback) 
            : base(binding, urlAddress)
        {
            this.binding = binding;
            this.urlAddress = urlAddress;
            this.queryParameters = queryParameters;
            this.clientCallback = clientCallback;
        }

        protected override ChannelFactory InitializeChannelFactory()
        {
            specificChannel = new DuplexChannelFactory<ISubscriber>(new InstanceContext(this), Binding, Endpoint);
            return specificChannel;
        }

        protected override void OpenChannel()
        {
            clientProxy = specificChannel.CreateChannel(Endpoint);
            Proxy = clientProxy as IClientChannel;
        }

        public void Subscribe(string topicName)
        {
            clientProxy.Subscribe(topicName);
        }

        public void Unsubscribe(string topicName)
        {
            clientProxy.Unsubscribe(topicName);
        }

        public override string Publish(TDataImage data)
        {
            if (clientCallback != null)
            {
                try
                {
                    clientCallback.OnDataReceived(data);
                }
                catch (Exception exception)
                {
                    clientCallback.OnError(exception);
                }
            }

            return null;
        }
    }
}