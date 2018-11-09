# kubeadm 安装 kubernetes

## Ubuntu 18.04.1 LTS

* SSR + polipo 科学上网：[https://www.jianshu.com/p/a0f3268bfa33](https://www.jianshu.com/p/a0f3268bfa33)

``` bash
export http_proxy="http://127.0.0.1:8123/"
export https_proxy="http://127.0.0.1:8123/"
```
* disable Swap 

``` bash
swapoff -a
sed -i '/ swap / s/^/#/' /etc/fstab
```

* install kubeadm: https://kubernetes.io/docs/setup/independent/install-kubeadm/

* docker http proxy: https://docs.docker.com/config/daemon/systemd/#httphttps-proxy

```
[Service]
Environment="HTTPS_PROXY=http://127.0.0.1:8123" "NO_PROXY=*.docker-cn.com,*.docker.com,*.yx.com"
```
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
cat << EOF > ~/kubeadm-master.yml
apiVersion: kubeadm.k8s.io/v1alpha3
kind: ClusterConfiguration
api:
  advertiseAddress: 0.0.0.0
networking:
  podSubnet: 10.244.0.0/16
EOF

kubeadm init --config ~/kubeadm-master.yml

# join node to cluster
kubeadm join 192.168.0.237:6443 --token 0zdquu.b37how3ijvvp5ba1 --discovery-token-ca-cert-hash sha256:20ee3b9557a1c36a2b857091d368d909467965ff100a06fb5cf7d99733dc5c32

mkdir -p $HOME/.kube
sudo cp -i /etc/kubernetes/admin.conf $HOME/.kube/config
sudo chown $(id -u):$(id -g) $HOME/.kube/config

# install pod network - Calico 
see: https://docs.projectcalico.org/v3.3/getting-started/kubernetes/

# on master
watch kubectl get nodes

# node2 NotReady - docker pull failed
journalctl -f -u kubelet

```

* Tear down 

```bash
kubectl drain kube-node2 --delete-local-data --force --ignore-daemonsets
kubectl delete node kube-node2
kubeadm reset
```

## Windows