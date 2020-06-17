# kubeadm 安装 kubernetes

## Ubuntu 18.04.1 LTS

* SSR + polipo 科学上网：[https://www.jianshu.com/p/a0f3268bfa33](https://www.jianshu.com/p/a0f3268bfa33)

``` bash
export http_proxy="http://192.168.0.248:1080/"
export https_proxy="http://192.168.0.248:1080/"
```
* disable Swap 

``` bash
sudo swapoff -a && sudo sed -i '/ swap / s/^/#/' /etc/fstab
```

* 多网卡主机需要指定网卡（k8s），k8s将作为flannel的参数

``` bash
# /etc/netplan/50-cloud-init.yaml
network:
    ethernets:
        k8s:
            match:
                macaddress: 00:50:56:99:62:a0
            addresses:
            - 172.16.0.21/24
            dhcp4: false
            gateway4: 172.16.0.1
            nameservers:
                addresses:
                - 192.168.0.201
                search:
                - local.com
            set-name: k8s
    version: 2

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
# 添加kubeadm配置
sudo kubeadm init --image-repository=registry.aliyuncs.com/google_containers --pod-network-cidr=10.244.0.0/16 --service-cidr=10.96.0.0/12 --apiserver-advertise-address=192.168.2.24 

rm -rf $HOME/.kube; \
mkdir -p $HOME/.kube; \
sudo cp -i /etc/kubernetes/admin.conf $HOME/.kube/config; \
sudo chown $(id -u):$(id -g) $HOME/.kube/config 

# setup flannel
wget https://raw.githubusercontent.com/coreos/flannel/bc79dd1505b0c8681ece4de4c0d86c5cd2643275/Documentation/kube-flannel.yml

name: kube-flannel
    image: quay.io/coreos/flannel:v0.10.0-arm64
    command:
    - /opt/bin/flanneld
    args:
    - --ip-masq
    - --kube-subnet-mgr
    - --iface="k8s" # 指定网卡

kubectl apply -f kube-flannel.yml


# join node to cluster
sudo kubeadm join 172.16.0.21:6443 --token 2tw8sk.b5cjhdmiasqwhqw4 --discovery-token-ca-cert-hash sha256:f4a195b6051513e5608682f75f1f165ca268693391ced9e81d5f123dfa08958e

# get token and discovery-token
kubeadm token list

# install pod network - Calico 
# see: https://docs.projetestalico.org/v3.3/getting-started/kubernetes/

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

### 双网卡节点

https://stackoverflow.com/questions/50401355/requests-timing-out-when-accesing-a-kubernetes-clusterip-service

* 修改 /etc/systemd/system/kubelet.service.d/10-kubeadm.conf
```
[Service]
Environment="KUBELET_KUBECONFIG_ARGS=--bootstrap-kubeconfig=/etc/kubernetes/bootstrap-kubelet.conf --kubeconfig=/etc/kubernetes/kubelet.conf --node-ip=192.168.0.245"
```

### failed to set bridge addr: "cni0" already has an IP address different from 10.244.1.1/24

在节点上：

```bash
sudo ip link delete cni0 && sudo ip link delete flannel.1
```


## Upgrade
