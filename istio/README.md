# istio

* OS: Ubuntu Server 18.04
* User: yang

## 部署
https://istio.io/zh/docs/setup/kubernetes/download-release/

``` bash
mkdir /opt/istio
cd /opt/istio
wget https://github.com/istio/istio/releases/download/1.0.0/istio-1.0.0-linux.tar.gz
tar -zxf istio-1.0.0-linux.tar.gz
echo "export PATH=/opt/istio/istio-1.0.0/bin:$PATH" > /etc/profile.d/istio.sh
source /etc/profile
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


## Greeter Sample

* 部署2个服务：greeter_server 和 api_server
* greeter_server: grpc服务，A bidirectional streaming RPC
* api_server：提供rest api，在api中调用greeter_server的服务
* LoadBalancer，设置最大并发量