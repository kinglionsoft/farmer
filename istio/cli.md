* kubeadm token create
* kubeadm token list
* 清除 flannel 网络
``` bash
sudo ifconfig  cni0 down && sudo brctl delbr cni0 && sudo ip link delete flannel.1
```