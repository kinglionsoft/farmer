# Elasticsearch on K8S

## 安装
https://www.elastic.co/guide/en/cloud-on-k8s/current/k8s-quickstart.html

``` bash

# from proxy
docker pull docker.elastic.co/eck/eck-operator:1.0.0-beta1 && docker tag docker.elastic.co/eck/eck-operator:1.0.0-beta1 registry.local.com/mirror/eck-operator:1.0.0-beta1 && docker push registry.local.com/mirror/eck-operator:1.0.0-beta1


kubectl apply -f https://download.elastic.co/downloads/eck/1.0.0-beta1/all-in-one.yaml

```

## 访问

PASSWORD=$(kubectl get secret quickstart-es-elastic-user -o=jsonpath='{.data.elastic}' | base64 --decode)