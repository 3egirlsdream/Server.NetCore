using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Server.NetCore.Commons;
using System;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Text;

namespace Server.NetCore.Domians
{
    public class RabbitMQDomain
    {
        IConnectionFactory factory;
        public RabbitMQDomain()
        {
            var host = SecretHelper.DESDecrypt("B0DF0F2682D51312C45E2C54B413B062");
            var user = SecretHelper.DESDecrypt("C3271FF5B151E8B9");
            var password = SecretHelper.DESDecrypt("C3271FF5B151E8B9");
            factory = new ConnectionFactory//创建连接工厂对象
            {
                HostName = host,//IP地址
                Port = 5672,//端口号
                UserName = user,//用户账号
                Password = password//用户密码
            };
        }
        public void Producter(string msg)
        {
            var conn = factory.CreateConnection();
            var channel = conn.CreateModel();
            string exchangeName = "exchange1";
            channel.ExchangeDeclare(exchangeName, type: "fanout");
            //消息内容
            byte[] body = Encoding.UTF8.GetBytes(msg);
            //发布消息,
            channel.BasicPublish(exchangeName, "", null, body);
            conn.Close();
            channel.Close();
        }

        public void Consumer()
        {
            var conn = factory.CreateConnection();
            var chanel1 = conn.CreateModel();
            string exchangeName = "exchange1";
            //声明交换机
            chanel1.ExchangeDeclare(exchangeName, ExchangeType.Fanout);
            //消息队列名称
            string queneName = "myqueue";
            //声明队列
            chanel1.QueueDeclare(queneName, false, true, true, null);
            //交换机和队列绑定
            chanel1.QueueBind(queneName, exchangeName, "", null);
            //定义消费者
            var consumer = new EventingBasicConsumer(chanel1);
            //接收事件
            consumer.Received += (model, ea) =>
            {
                byte[] message = ea.Body.ToArray();//接收到的消息
                Debug.WriteLine($"接收到信息为:{Encoding.UTF8.GetString(message)}");
                //返回消息确认
                chanel1.BasicAck(ea.DeliveryTag, true);
            };
            //开启监听
            chanel1.BasicConsume(queneName, false, consumer);
            Debug.WriteLine($"队列名称:{queneName}");
        }
    }
}
