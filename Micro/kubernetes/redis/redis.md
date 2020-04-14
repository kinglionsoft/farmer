# Redis on K8S

## 需求
* 高可用
* 无需持久化
* 集群内部访问，无需认证

## 安装

```bash
kubectl create configmap redis-config --from-file ./redis-config

kubectl apply -f redis.yaml
```
## 连接

### 集群内

``` bash
redis:6379
```

### 集群外测试

``` bash
kubectl port-forward redis --address=0.0.0.0 6379

redis-cli -h 172.16.0.21
```