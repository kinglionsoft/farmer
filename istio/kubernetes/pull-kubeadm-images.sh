# 从 registry.yx.com 拉取镜像
images=( "k8s.gcr.io/coredns:1.2.2" "k8s.gcr.io/etcd:3.2.24" "k8s.gcr.io/kube-apiserver:v1.12.2" "k8s.gcr.io/kube-controller-manager:v1.12.2" "k8s.gcr.io/kube-proxy:v1.12.2" "k8s.gcr.io/kube-scheduler:v1.12.2" "k8s.gcr.io/pause:3.1" "quay.io/calico/cni:v3.3.0" "quay.io/calico/kube-controllers:v3.3.0" "quay.io/calico/node:v3.3.0" "quay.io/coreos/etcd:v3.3.9")

registry="registry.yx.com/"

for img in ${images[@]};
do
    docker pull $registry$img
    docker tag $registry$img $img
	docker rmi $registry$img
done 

# for img in ${images[@]}; do docker rmi $registry$img; done