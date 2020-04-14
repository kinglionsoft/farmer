# RabbitMQ

* 无持久化需求
* 混合云外部访问 5672 <==> 35672：文件处理服务部署在Windows Server中，不在k8s集群内。
* 集群中的每个rabbitmq都使用相同的cookie

```bash
echo $(openssl rand -base64 32) > erlang.cookie
kubectl create secret generic erlang.cookie --from-file=erlang.cookie 
kubectl apply -f rabbitmq.yaml

```

* 开启k8s代理后，可以临时访问集群管理界面 http://m.k8s.local.com:15672

```bash
kubectl port-forward rabbitmq-bd5797cfc-z6vj6 --address=0.0.0.0 15672
```