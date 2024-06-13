# OpenMediaVault

## 安装



## docker

https://docs.docker.com/engine/install/debian/

## RAID

#### 创建RAID，应用更改时报错

https://github.com/openmediavault/openmediavault/issues/1665
```
omv-confdbadm read --defaults "conf.system.notification.notification" | jq ".id = \"mdadm\"" | omv-confdbadm update "conf.system.notification.notification" -
```