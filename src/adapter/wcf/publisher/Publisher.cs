using System.ServiceModel;
using System.ServiceModel.Channels;

namespace wcf.communication.adapter.wcf.publisher
{
    public class Publisher<TDataImage> : AInternalAdapter<TDataImage>
    {
        private readonly string topicName;
        private readonly string partitionName;
        private ChannelFactory<IPublisher<TDataImage>> specificChannel;
        private IPublisher<TDataImage> clientProxy;

        public Publisher(Binding binding, string urlAddress, string topicName, string partitionName)
            : base(binding, urlAddress)
        {
            this.topicName = topicName;
            this.partitionName = partitionName;
        }

        protected override ChannelFactory InitializeChannelFactory()
        {
            specificChannel = new ChannelFactory<IPublisher<TDataImage>>(Binding, Endpoint);
            return specificChannel;
        }

        protected override void OpenChannel()
        {
            clientProxy = specificChannel.CreateChannel(Endpoint);
            Proxy = clientProxy as IClientChannel;
        }

        public override string Publish(TDataImage data)
        {
            return clientProxy.Publish(data, topicName, partitionName);
        }
    }
}