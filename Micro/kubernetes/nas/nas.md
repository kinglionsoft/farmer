# NAS
使用NAS(NFS)作为镜像的挂载盘，实现文件共享。

## 测试环境
* Windows Server 2019 作为NFS server
https://blog.bobbyallen.me/2018/01/18/windows-server-2016-as-an-nfs-server-for-linux-clients/

* docker
Ubuntu 宿主上安装nfs-common

``` bash
sudo apt install -y nfs-common
```

* k8s

``` bash
kubectl apply -f - <<EOF
apiVersion: v1
kind: PersistentVolume
metadata:
  name: nfs-share
spec:
  storageClassName: manual
  capacity:
    storage: 200Gi
  persistentVolumeReclaimPolicy: Retain
  accessModes:
    - ReadWriteMany
  nfs:
    server: nas
    path: "/k8s/ctc"
EOF

kubectl apply -f - <<EOF
kind: PersistentVolumeClaim
apiVersion: v1
metadata:
  name: ctc-share
spec:
  storageClassName: manual
  accessModes:
    - ReadWriteMany
  resources:
    requests:
      storage: 1000Gi
EOF

kubectl apply -f - <<EOF
apiVersion: v1
kind: PersistentVolume
metadata:
  name: pv-nfs-static
spec:
  storageClassName: ""
  capacity:
    storage: 50Gi
  persistentVolumeReclaimPolicy: Retain
  accessModes:
    - ReadOnlyMany
  nfs:
    server: nas
    path: "/k8s/ctc-static"
---
kind: PersistentVolumeClaim
apiVersion: v1
metadata:
  name: ctc-static
spec:
  storageClassName: ""
  volumeName: pv-nfs-static
  accessModes:
    - ReadOnlyMany
  resources:
    requests:
      storage: 50Gi
EOF


# test
kubectl create -f - <<EOF
apiVersion: v1
kind: Pod
metadata:
  name: pod-nfs-test
spec:
  containers:
  - name: aspnet
    image: registry.local.com/dotnetcore/runtime-gdi:3.0
    command: ["/bin/sh", "-c", "while true; do du /home/html -sh ; sleep 10;done"]       
    volumeMounts:
    - name: html
      mountPath: "/home/html/"
  volumes:
  - name: html
    persistentVolumeClaim:
      claimName: ctc-share
EOF

# test
kubectl logs pod-nfs-test

# delete
kubectl delete pod pod-nfs-test
```

### 生成环境

阿里云建立2类存储：性能型、容量型。AWS对应的产品是EFS

#### 性能型
用于容器快速共享文件

#### 容量型
用于存放静态文件，如：客户端安装包

#### PV & PVC扩容

##### 离线扩容
停止pod后，重建PVC
```bash

kubectl patch pvc PVC_NAME -p '{"metadata":{"finalizers": []}}' --type=merge
kubectl delete pvc PVC_NAME 

kubectl edit pv PV_NAME # delete claimRef

# recreate PVC
```

#####  在线扩容
1. 扩容NAS
2. 修改PV: spec.capacity.storage
3. 创建或者修改StorageClass
``` bash
kubectl create -f - <<EOF
apiVersion: storage.k8s.io/v1
kind: StorageClass
metadata:
  name: nas
parameters:
  type: cloud_ssd
provisioner: alicloud/disk
reclaimPolicy: Delete
volumeBindingMode: Immediate
allowVolumeExpansion: true # 允许扩容
EOF
```
4. 修改PVC: spec.resources.requests.storage
5. PVC处于FileSystemResizePending，直到挂载的pod重启

> 阿里云回复：基于阿里云NAS的PVC不受PVC声明容量的限制，只需要扩容NAS盘。

#### 同步历史文件

* 将Windows Server 2019上到的历史文件夹开启NFS共享。
* 在阿里云内网的Linux主机上同时mount历史文件夹和NAS盘：

```bash
# 按年进行同步
rsync -q -r -u --exclude=2019* --exclude=2020*  k8s/ k8s2/
rsync -q -r -u --exclude=2018* --exclude=2020*  k8s/ k8s2/
rsync -q -r -u --exclude=2018* --exclude=2019*  k8s/ k8s2/
```

### PV/PVC删除

删除PV、PVC后一直处于terminating.

``` bash
kubectl patch pvc pvc_name -p '{"metadata":{"finalizers":null}}'
kubectl patch pv pv_name -p '{"metadata":{"finalizers":null}}'
kubectl patch pod pod_name -p '{"metadata":{"finalizers":null}}'
```