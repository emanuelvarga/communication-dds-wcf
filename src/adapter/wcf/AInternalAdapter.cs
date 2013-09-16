using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using communication.dds.api.adapter.publisher;

namespace wcf.communication.adapter.wcf
{
    public abstract class AInternalAdapter<TDataImage> : DataPublisher<TDataImage>
    {
        protected ChannelFactory Channel;
        protected IClientChannel Proxy;
        
        protected readonly Binding Binding;

        protected EndpointAddress Endpoint;

        protected AInternalAdapter(Binding binding, string address)
        {
            if (binding == null)
            {
                throw new ArgumentNullException("binding");
            }

            if (string.IsNullOrEmpty(address))
            {
                throw new ArgumentNullException("address");
            }

            Binding = binding;
            Endpoint = new EndpointAddress(address);

            PrepareConnection();
        }

        protected bool ConnectionOpen
        {
            get { return IsChannelOpen && IsProxyConnected; }
        }

        private bool IsChannelOpen
        {
            get
            {
                return Channel != null &&
                       Channel.State != CommunicationState.Faulted &&
                       Channel.State == CommunicationState.Opened;
            }
        }

        private bool IsProxyConnected
        {
            get
            {
                var clientChannelProxy = Proxy as IClientChannel;
                return clientChannelProxy != null &&
                       clientChannelProxy.State != CommunicationState.Faulted &&
                       clientChannelProxy.State != CommunicationState.Opened;
            }
        }

        protected abstract ChannelFactory InitializeChannelFactory();

        protected abstract void OpenChannel();

        protected bool PrepareConnection()
        {
            if (!IsChannelOpen)
            {
                Channel = InitializeChannelFactory();
            }

            if (!IsChannelOpen)
            {
                OpenChannel();
                Proxy.Open();
            }

            return true;
        }

        public override void Dispose()
        {
            try
            {
                var clientChannelProxy = Proxy;
                if (clientChannelProxy != null)
                {
                    clientChannelProxy.Dispose();
                    clientChannelProxy = null;
                }

                if (Channel != null)
                {
                    Channel.Close();
                    Channel = null;
                }
            }
            catch
            {
            }
        }
    }
}