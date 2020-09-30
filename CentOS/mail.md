# Send Email

## Install

``` bash
yum install mailx -y

```

## Configuration

```shell
vi /etc/mail.rc
```
edit
```
set smtp=smtps://smtp.qq.com:465
set smtp-auth=login
set from=xx@qq.com
set smtp-auth-user=xx@qq.com
set smtp-auth-password=szrfeivwgmccafc
set ssl-verify=ignore
set nss-config-dir=/etc/pki/nssdb/
```
example usage :

```shell
echo "Your message" | mail -v -s "Message Subject" email@address
```
