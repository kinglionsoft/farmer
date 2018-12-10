# 部署 k8s Dashboard

* https://k8smeetup.github.io/docs/tasks/access-application-cluster/web-ui-dashboard/

### 配置用户名密码

``` bash
cat << EOF > dashboard-user.yaml
# dashboard-user
apiVersion: v1
kind: ServiceAccount
metadata:
  name: admin-user
  namespace: kube-system
---
# admin-user-binding
apiVersion: rbac.authorization.k8s.io/v1beta1
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
---
# kubernetes-dashboard remote access
apiVersion: rbac.authorization.k8s.io/v1beta1
kind: ClusterRoleBinding
metadata:
  name: kubernetes-dashboard
  labels:
    k8s-app: kubernetes-dashboard
roleRef:
  apiGroup: rbac.authorization.k8s.io
  kind: ClusterRole
  name: cluster-admin
subjects:
- kind: ServiceAccount
  name: kubernetes-dashboard
  namespace: kube-system

EOF
kubectl create -f dashboard-user.yaml

# 查看token
kubectl -n kube-system describe secret $(kubectl -n kube-system get secret | grep admin-user | awk '{print $1}')

# 启动代理 k8s.yx.com -> 192.168.0.237
kubectl proxy --address=192.168.0.237 --accept-hosts=^k8s\.yx\.com
```
* 浏览器访问：http://k8s.yx.com:8001/api/v1/namespaces/kube-system/services/https:kubernetes-dashboard:/proxy/#!/login
