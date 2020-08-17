# Redash on K8S

## Helm
* https://github.com/getredash/contrib-helm-chart

## Localization
* 不支持国际化

## Create Users

```bash
kubectl exec ytzx-redash-76bfb699ff-wqp7b -- ./bin/run ./manage.py users create --admin --password '<password>' xxx@qq.com '<name>'
```