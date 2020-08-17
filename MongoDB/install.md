# MongoDB Installation

## CentOS 8.1

* https://docs.mongodb.com/manual/tutorial/install-mongodb-on-red-hat/

## Security

```bash
use admin
db.createUser({user:"admin",pwd:"xxxxxxx",roles:["root"]})
db.createUser({user:"dev",pwd:"xxx",roles:["root"]})
```