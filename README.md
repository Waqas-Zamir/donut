# Donut #

API all the things!

### How to configure

```
# apis
auth apis add users_api secret -d "Web Terminal (Users) Web API (Donut)" -c name -c role -c username -v
auth apis add accounts_api secret -d "Web Terminal (Accounts) Web API (Donut)" -c name -c role -c username -v
auth apis add donut_api secret -d "Web Terminal Web API (Donut)" -c name -c role -c username -a donut_api -a users_api -a accounts_api -v

# clients
auth clients add console donut_console -n "Web Terminal (Donut) Web API" -r http://127.0.0.1 -a openid -a profile -a email -a users_api -a accounts_api -o -k  -p -q -v

# users
auth users add donut -p integration -v
```