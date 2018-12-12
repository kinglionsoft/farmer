# kubeadm 安装 kubernetes

## Ubuntu 18.04.1 LTS

* SSR + polipo 科学上网：[https://www.jianshu.com/p/a0f3268bfa33](https://www.jianshu.com/p/a0f3268bfa33)

``` bash
export http_proxy="http://192.168.0.237:8123/"
export https_proxy="http://192.168.0.237:8123/"
```
* disable Swap 

``` bash
sudo swapoff -a && sudo sed -i '/ swap / s/^/#/' /etc/fstab
```

* 创建用户

``` bash
sudo adduser k8s
sudo addgroup wheel
sudo addgroup k8s wheel
sudo visudo # 设置wheel组sudo不需要密码
```

* install kubeadm: https://kubernetes.io/docs/setup/independent/install-kubeadm/

* docker http proxy: https://docs.docker.com/config/daemon/systemd/#httphttps-proxy

* config master

``` bash
# 提前拉去镜像
kubeadm config images pull

[config/images] Pulled k8s.gcr.io/kube-apiserver:v1.12.2
[config/images] Pulled k8s.gcr.io/kube-controller-manager:v1.12.2
[config/images] Pulled k8s.gcr.io/kube-scheduler:v1.12.2
[config/images] Pulled k8s.gcr.io/kube-proxy:v1.12.2
[config/images] Pulled k8s.gcr.io/pause:3.1
[config/images] Pulled k8s.gcr.io/pause:3.1
[config/images] Pulled k8s.gcr.io/etcd:3.2.24
[config/images] Pulled k8s.gcr.io/coredns:1.2.2

# 添加kubeadm配置
kubeadm init --pod-network-cidr=10.244.0.0/16 --service-cidr=10.96.0.0/12

mkdir -p $HOME/.kube
sudo cp -i /etc/kubernetes/admin.conf $HOME/.kube/config
sudo chown $(id -u):$(id -g) $HOME/.kube/config

# join node to cluster
sudo kubeadm join 192.168.0.237:6443 --token pq3asj.6apo1p3kac44px6x --discovery-token-ca-cert-hash sha256:7a483421420d26d60572939ee1f35b2ff2c5a8c2f1e87d35c3c5075150ad12eb

mkdir -p $HOME/.kube; \
sudo cp -i /etc/kubernetes/admin.conf $HOME/.kube/config; \
sudo chown $(id -u):$(id -g) $HOME/.kube/config 

# get token and discovery-token
kubeadm token list

# install pod network - Calico 
# see: https://docs.projectcalico.org/v3.3/getting-started/kubernetes/

# install pod network - Fannel - for working with Windows


# on master
watch kubectl get nodes

# node2 NotReady - docker pull failed
kubectl describe nodes
journalctl -f -u kubelet

```

* Tear down 

```bash
kubectl drain kube-node1 --delete-local-data --force --ignore-daemonsets
kubectl delete node kube-node1
kubeadm reset
```

## Dashboard
* https://kubernetes.io/docs/tasks/access-application-cluster/web-ui-dashboard/#accessing-the-dashboard-ui

## Errors

#### ImagePullBackOff

``` bash
kubectl describe pod kubernetes-dashboard-77fd78f978-xbnfb -n=kube-system # 查看日志 
# Failed to pull image "k8s.gcr.io/kubernetes-dashboard-amd64:v1.10.0" : 科学上网
```

