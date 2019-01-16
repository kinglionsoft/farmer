# 升级

``` bash
# set ssr: 直连模式
export http_proxy=http://192.168.0.123:1080
export https_proxy=http://192.168.0.123:1080

# set proxy for apt
tee apt-proxy.conf <<EOF
Acquire::http::proxy "http://192.168.0.123:1080/";
Acquire::https::proxy "http://192.168.0.123:1080/";
EOF

# upgrade kubeadm
sudo apt update -c apt-proxy.conf && \
sudo apt-mark unhold kubeadm && \
sudo apt install -y kubeadm=1.13.1-00 -c apt-proxy.conf && \
sudo apt-mark hold kubeadm

# set proxy for docker

# upgrade cluster
kubeadm upgrade apply v1.13.1

# upgrade kubelet
sudo apt-mark unhold kubelet && \
sudo apt install -y kubelet=1.13.1-00 -c apt-proxy.conf && \
sudo apt-mark hold kubelet

# ugrade kubectl on all nodes
sudo apt-mark unhold kubectl && \
sudo apt install -y kubectl=1.13.1-00 -c apt-proxy.conf && \
sudo apt-mark hold kubectl

# upgrade the kubelet config on worker nodes
sudo kubeadm upgrade node config --kubelet-version v1.13.1

# Restart the kubelet for all nodes
sudo systemctl restart kubelet

```
