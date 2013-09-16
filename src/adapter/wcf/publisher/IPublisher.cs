using System.ServiceModel;

namespace wcf.communication.adapter.wcf.publisher
{
    [ServiceContract]
    public interface IPublisher<TDataImage>
    {
        [OperationContract(IsOneWay = true)]
        string Publish(TDataImage message, string topicName, string partitionName);
    }
}