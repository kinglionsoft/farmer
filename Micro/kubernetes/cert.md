# Renew certificates

* 有配置文件

```bash
sudo kubeadm alpha certs renew all --config=/home/dev/.kube/kubeadm.yaml
sudo cp -a /etc/kubernetes/admin.conf ~/.kube/config
sudo chown dev:dev ~/.kube/config
```

* 无备份文件

``` bash
sudo kubeadm alpha  certs renew all
sudo service kubelet restart
sudo cp -a /etc/kubernetes/admin.conf ~/.kube/config
sudo chown dev:dev ~/.kube/config

# restart core docker container
docker ps |grep -E 'k8s_kube-apiserver|k8s_kube-controller-manager|k8s_kube-scheduler|k8s_etcd_etcd'|xargs docker restart

# backup
kubeadm config view > /home/dev/.kube/kubeadm.yaml
```