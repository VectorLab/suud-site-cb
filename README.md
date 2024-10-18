# SUUD SSO example project ( .NET & React )

## Overview
This project was edited by VectorLab, it is used for connection of United Community Website to SUUD ( Simple Universal User Dashboard ).

## Project Structure
```
.
├── back (backend)
│   ├── Controllers
│   │   └── SignInController.cs
│   ├── Utils
│   │   └── SsoSuud.cs
│   └── appsettings.json
├── devproxy
│   └── dev-proxy-s.js
└── front
    └── src
        └── pages
            ├── Index.tsx
            └── Home.tsx (sso login result handler)
```
## Site Key Setup
1. **back/appsettings.json**: Site ID & PrivateKey & Password here in SsoSuud section
2. **front/src/pages/Index.tsx**: Site ID here in ssoClientID variable
3. **devproxy/dev-proxy-s.js**: Development site SSL Certificate here

## Key Components
1. **back/Controllers/SignInController.cs**: site login request handler for client
2. **back/Utils/SsoSuud.cs**: SSO request request handler for suud.net
3. **front/src/pages/Index.tsx**: SSO process launcher
4. **front/src/pages/Home.tsx**: SSO response receiver

## Deploy
### back (backend)
```bash
dotnet restore
dotnet build -c Release
dotnet ./bin/Release/net8.0/suud-site-cb.dll
```

### front (frontend)
```bash
npm run build
```
Then, uses ```build``` directory as static web resources.

### nginx (proxy)
```
server {
    listen       443 ssl;
    http2 on;
    server_name  example.com www.example.com;

    ssl_certificate      "/.../cert.pem";
    ssl_certificate_key  "/.../key.pem";

    ssl_session_cache    shared:SSL:1m;
    ssl_session_timeout  10m;

    ssl_ciphers  HIGH:!aNULL:!MD5;
    ssl_prefer_server_ciphers  on;
    ssl_protocols TLSv1.3;

    if ($ssl_protocol = "") { return 301 https://$host$request_uri; }

    location /api/ {
        proxy_pass http://localhost:5000/;
        proxy_redirect off;

        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;

        proxy_set_header Host $http_host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Nginx-Proxy true;
    }

    location / {
        root /home/worker/react/build;
        try_files $uri $uri/ /index.html;
    }
}
```

## Testing
- When opening path / , application will show enter button.
- Click this button, the application will jump to suud.net and complete the verification.
- If success, application will jump to /home and show the user who is login.
- If fail, application will jump to / which is same as it beginning. 

## Notice
- Both Private Key and Password should not be exposed to public in client side.
- Always use HTTPS with the domain consistent with Password.
