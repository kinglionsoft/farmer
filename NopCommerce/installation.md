# 安装

## 问题

### 中文语言包乱码
数据库为MariaDB, 字符集为utf8mb4。表LocaleStringResource中的中文为乱码。

#### 解决方案
安装时，手动指定数据库连接字符串，并设置**charset**

```
server=xx;database=mall;allowuservariables=True;user id=dev;password=1234;charset=utf8mb4
```
