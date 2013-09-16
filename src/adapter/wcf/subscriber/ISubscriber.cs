using System.ServiceModel;
using wcf.communication.adapter.wcf.publisher;

namespace wcf.communication.adapter.wcf.subscriber
{
    [ServiceContract(CallbackContract = typeof(IPublisher<>))]
    public interface ISubscriber
    {
        [OperationContract]
        void Subscribe(string topicName);

        [OperationContract]
        void Unsubscribe(string topicName);
    }
}
