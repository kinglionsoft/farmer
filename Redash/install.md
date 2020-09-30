# Redash on K8S

## Helm
* https://github.com/getredash/contrib-helm-chart

## Localization
* 不支持国际化

## Create Users

```bash
# admin
kubectl exec ytzx-redash-76bfb699ff-4q9rt -- ./bin/run ./manage.py users create --admin --password '123456' xxx2@qq.com 'xx'
# defaul
kubectl exec ytzx-redash-76bfb699ff-4q9rt -- ./bin/run ./manage.py users create --password '123456' xx@qq.com 'xx'
# delete
kubectl exec ytzx-redash-76bfb699ff-4q9rt -- ./bin/run ./manage.py users delete xxx@qq.com

```