using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Server.NetCore.Commons
{
    public static class RabbitMQService 
    {
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConnectionFactory factory)
        {
            var conn = factory.CreateConnection();
            var chanel1 = conn.CreateModel();
            string exchangeName = "exchange1";
            //声明交换机
            chanel1.ExchangeDeclare(exchangeName, ExchangeType.Fanout);
            //消息队列名称
            string queneName = DateTime.Now.Second.ToString();
            //声明队列
            chanel1.QueueDeclare(queneName, false, false, false, null);
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
            return services;
        }
    }
}
