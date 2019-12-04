# 部署 k8s Dashboard

* https://github.com/kubernetes/dashboard

### 配置用户名密码

``` bash
kubectl create -f - <<EOF
apiVersion: v1
kind: ServiceAccount
metadata:
  name: admin-user
  namespace: kube-system
---
apiVersion: rbac.authorization.k8s.io/v1
kind: ClusterRoleBinding
metadata:
  name: admin-user
roleRef:
  apiGroup: rbac.authorization.k8s.io
  kind: ClusterRole
  name: cluster-admin
subjects:
- kind: ServiceAccount
  name: admin-user
  namespace: kube-system
EOF
```

### 访问

使用K8s API Server的方式访问， 参考： https://www.cnblogs.com/RainingNight/p/deploying-k8s-dashboard-ui.html

#### 生成证书
``` bash
# 生成client-certificate-data
grep 'client-certificate-data' ~/.kube/config | head -n 1 | awk '{print $2}' | base64 -d >> kubecfg.crt
# 生成client-key-data
grep 'client-key-data' ~/.kube/config | head -n 1 | awk '{print $2}' | base64 -d >> kubecfg.key
# 生成p12
openssl pkcs12 -export -clcerts -inkey kubecfg.key -in kubecfg.crt -out kubecfg.p12 -name "kubernetes-client"

# 下载证书并安装 kubecfg.p12 密码123456
```

#### 登录

https://172.16.0.21:6443/api/v1/namespaces/kube-system/services/https:kubernetes-dashboard:/proxy/

``` bash
# token
kubectl -n kube-system describe secret $(kubectl -n kube-system get secret | grep admin-user | awk '{print $1}')
```
