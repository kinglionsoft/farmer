# Kubernetes 部署

* OS: Ubuntu Server 18.04 
* User: root

## 安装 kubernetes

* 参考 [https://www.kubernetes.org.cn/4387.html](https://www.kubernetes.org.cn/4387.html)
* Master: 192.168.237
* Node1: 192.168.0.238
* Node2: 192.168.0.239
* Win Node1: 192.168.0.243

### 补充

* Master上安装成功的信息为：

``` bash
Your Kubernetes master has initialized successfully!

To start using your cluster, you need to run the following as a regular user:

  mkdir -p $HOME/.kube
  sudo cp -i /etc/kubernetes/admin.conf $HOME/.kube/config
  sudo chown $(id -u):$(id -g) $HOME/.kube/config

You should now deploy a pod network to the cluster.
Run "kubectl apply -f [podnetwork].yaml" with one of the options listed at:
  https://kubernetes.io/docs/concepts/cluster-administration/addons/

You can now join any number of machines by running the following on each node
as root:

  kubeadm join 192.168.0.240:6443 --token sfntx3.td9aoc1f2puj8l9m --discovery-token-ca-cert-hash sha256:3eb9900ffa7dfb3bd38ccad62b59db21eb75342f04d2abb5c02ac1399812fc26
```
在Node上执行kubeadm join后，在master上可以看到所有节点：

``` bash
kubectl get nodes
```

### Errors
* certificate apiserver-kubelet-client is not signed by corresponding CA

``` bash
sudo rm -rf /var/lib/localkube/certs
```

* a kubeconfig file "/etc/kubernetes/admin.conf" exists already but has got the wrong CA cert

``` bash
kubeadm reset
sudo minikube delete
sudo rm -rf ~/.minikube
```

* Port 10250 is already in use

``` bash
sudo kubeadm reset
```

* Starting cluster components... 长时间卡住
原因未知，删除重来

* docker 添加私有registry
写入：/etc/docker/daemon.json

``` js
{
  "registry-mirrors": ["https://78i6a9m9.mirror.aliyuncs.com"],
  "insecure-registries": ["192.168.0.240:5000"]
}
```

### 删除

``` bash
```

