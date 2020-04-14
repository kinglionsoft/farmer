# Kafka on K8S

## 需求
* 单节点

## 安装

* 准备镜像

``` bash
docker pull wurstmeister/kafka && docker tag wurstmeister/kafka registry.local.com/mirror/kafka && && docker push registry.local.com/mirror/kafka
```

## 访问
* 集群外：<集群IP>:39092
* 集群内：kafka:9092