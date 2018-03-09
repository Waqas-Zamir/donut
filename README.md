# Donut #

API all the things!

### How to configure

```
auth apis add donut_api secret -d "Donut Web API (introspection)"
auth apis add users_api secret -d "Web Terminal (Users) Web API (Donut)" -c sub -v
auth apis add accounts_api secret -d "Web Terminal (Accounts) Web API (Donut)" -c sub -v
auth clients add console donut_console -n "Web Terminal (Donut) Web API" -r http://127.0.0.1 -a openid -a profile -a email -a users_api -a accounts_api -o -k  -p -q -v
```