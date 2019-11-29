# istio

* OS: Ubuntu Server 18.04
* User: yang

## 部署
* see: https://istio.io/docs/setup/kubernetes/
* 修改 istio-demo.yaml，这是nodeSelector为linux。修改后文件为：istio-demo-win.yaml
``` bash
cd /opt

```

## 配置

* 手动注入sidecar

``` bash
istioctl kube-inject -f greeter.yaml | kubectl apply -f -
``` 

* master 常用指令

``` bash
# 查看所有节点\部署\服务\pods\hpa(水平伸缩))
kubectl get nodes|deployment|services|pods|hpa

# 查看节点\部署\服务\pod运行情况
kubectl describe nodes|deployment|services|pods

# 查看pod日志
kubectl logs <pod-name>
kubectl -n ${NAMESPACE} describe pod ${POD_NAME}
kubectl -n ${NAMESPACE} logs ${POD_NAME} -c ${CONTAINER_NAME}

# 自动扩容
kubectl autoscale deployment <deployment-name> --cpu-percent=50 --min=1 --max=2

``` 

## 坑

* 1: 网络原因，无法拉取部分docker image

解决方案：从阿里云等镜像库中拉取同名同版本的image到本地，再修改tag为.yaml中配置的tag

* 2: docker 中添加阿里云镜像及本地私有镜像

/etc/docker/daemon.json
{
  "registry-mirrors": ["https://78i6a9m9.mirror.aliyuncs.com"],
  
  "insecure-registries": ["192.168.0.240:5000"]
}

* 3. External IP
本地集群没有LB，通过nginx代理实现

``` bash
# 获取istio-ingressgateway的nodePort
kubectl -n istio-system get service istio-ingressgateway -o jsonpath='{.spec.ports[?(@.name=="http2")].nodePort}'

# nginx

server {
  listen       80;

  location / {
    proxy_pass http://127.0.0.1:31380/;
    proxy_http_version 1.1;
    proxy_set_header Connection "";
    proxy_set_header Host $http_host;
    }
}
```

## Greeter Sample

* 部署2个服务：greeter_server 和 api_server
* greeter_server: grpc服务，A bidirectional streaming RPC
* api_server：提供rest api，在api中调用greeter_server的服务
* LoadBalancer，设置最大并发量