# Unity3D+C#服务器+MySQL网络游戏开发

**版本计划：**

- [x] Version1：项目完整运行，全程Socket连接。对应分支V1.0
- [x] Version2：简单修改UI，确定完整的逻辑开发流程。初步修改。对应分支V2.0
- [x] Version3：根据逻辑开发流程开发。对应分支V3.0
- [ ] Version4：战斗逻辑开发。对应分支V4.0
- [ ] Version5：细节优化等
#### 流程图
```mermaid
graph TD
    A[启动客户端] --> B[自动Socket连接]
    B -->|成功| C[启动Ping/Pong心跳检测]
    B -->|失败| D[循环重试连接]
    
    E[输入账号密码] --> F{Socket已连接?}
    F -->|是| G[HTTP Post登录]
    F -->|否| H[阻止登录并提示]
    
    G --> I[登录成功]
    I --> J[发送绑定用户协议]
    J --> K[服务器绑定UserID和Socket]
    
    K --> L{账号是否已登录?}
    L -->|是| M[服务器Socket发送踢下线协议]
    L -->|否| N[直接登录]
    M --> O[旧客户端退回登录界面]
    
    N --> P[HTTP Get头像]
    
    Q[开始游戏]
    
    Q --> R[客户端Socket发送获取房间列表协议]
    R --> S[Unity获取所有房间并加载]
    
    S --> T[创建房间]
    T --> U[客户端Socket发送创建房间协议，进入房间]
    S --> V[进入房间]
    V --> W[客户端Socket发送进入房间协议，进入房间]
    W --> X[退出房间]
    
    X --> X1[客户端Socket发送离开房间协议]
    X1 --> X2{房间人数是否为0}
    X2 -->|是| X3[服务器广播发送删除房间协议]
    X2 -->|否| X4[服务器对房间的Socekt发送玩家离开房间协议]
    
    W --> Y[进入游戏]
    Y --> Z1[客户端Socket发送开始战斗协议]
    Z1 --> Z3[服务器接收开始战斗协议并转发给其他客户端]
    Z1 --> Z2[进入加载界面,加载完成发送加载完成协议]
    Z3 --> Z2[进入加载界面,加载完成发送加载完成协议]
    Z2 -->|服务器收到加载完成协议数==等于房间数玩家| Z4[服务器发送进入战斗协议]
    Z2 -->Z5[收到一个加载完成协议后等待10秒]
    Z5 --> Z4[服务器发送进入战斗协议]
   	Z4 --> Z6[客户端进入游戏]
    
    Z6 --> AA[游戏结束]
    AA --> AB[Socket发送结束战斗协议]
    AB --> AC[服务端同步数据]
    AC --> AD[HTTP Get用户信息]
    AD --> AE[更新UI数据]
    
    style A fill:#f9f,stroke:#333
    style E fill:#bbf,stroke:#333
    style R fill:#9f9,stroke:#333
    style Y fill:#f96,stroke:#333
    style AA fill:#f99,stroke:#333
```
#### 登录
```mermaid
sequenceDiagram
    participant Untiy
    participant 服务器
    participant 数据库
    
    Untiy->>服务器: POST /api/login
    服务器->>数据库: 查询用户
    数据库-->>服务器: 返回用户数据
    alt 用户存在
        服务器->>服务器: 验证密码哈希
        alt 密码正确
            服务器->>数据库: 更新最后登录时间
            服务器->>服务器: 验证用户是否已经登录
            alt 没有登录
            服务器->>服务器: 添加用户信息
            else 已经登录
            服务器-->>Untiy: 踢下线协议
            end
            服务器-->>Untiy: 200 + 用户数据
            Untiy-->>服务器: 客户端发送绑定User协议
        else 密码错误
            服务器-->>Untiy: 401 错误
        end
    else 用户不存在
        服务器-->>Untiy: 401 错误
    end
```
#### 服务器HTTP
```mermaid
graph LR
A[请求入口] --> B[中间件1]
B --> C[中间件2]
C --> D[...]
D --> E[路由处理]
E --> F[响应返回]
```
```mermaid
sequenceDiagram
    participant Unity
    participant HttpListener
    participant MiddlewarePipeline
    participant RouteTable
    participant AuthController
    
    Unity->>HttpListener: HTTP Request
    HttpListener->>MiddlewarePipeline: 传递Context
    MiddlewarePipeline->>ExceptionMiddleware: 异常捕获
    MiddlewarePipeline->>CorsMiddleware: 跨域处理
    MiddlewarePipeline->>RouteTable: 路由分发
    RouteTable->>AuthController: 调用对应方法
    AuthController-->>RouteTable: 返回结果
    RouteTable-->>MiddlewarePipeline: 响应数据
    MiddlewarePipeline-->>HttpListener: 返回处理结果
    HttpListener-->>Unity: HTTP Response
```
#### 头像上传流程
```mermaid
sequenceDiagram
Unity客户端->>+HTTP服务器: POST /api/upload (multipart/form-data)
HTTP服务器->>+文件存储: 保存图片到/uploads/user_123/abc.webp
HTTP服务器->>+MySQL: 插入记录(id=1, user_id=123, path='uploads/user_123/abc.webp')
HTTP服务器-->>-Unity客户端: 返回{url: '/cdn/uploads/user_123/abc.webp'}
```
#### 头像下载流程
```mermaid
sequenceDiagram
Unity客户端->>+HTTP服务器: GET /api/images/1
HTTP服务器->>+MySQL: 查询记录WHERE id=1
MySQL-->>-HTTP服务器: 返回path
HTTP服务器->>+文件存储: 读取文件数据
文件存储-->>-HTTP服务器: 返回图片字节流
HTTP服务器-->>-Unity客户端: 返回图片(200 OK with image/webp)
```
#### 打开房间大厅

```mermaid
sequenceDiagram
    participant Client
    participant Server
    Client->>Server: GET /rooms (初始加载)
    Server->>Client: 200 OK [所有房间数据]
    Client->>Server: 订阅room_updates
    loop 实时监听
        Server->>Client: 服务器推送{"action":"delete","roomId":123}
        Server->>Client: 服务器推送{"action":"create","room":{...}}
        Client->>Client: 从本地缓存移除房间123
        Client->>Client: 从本地缓存新增房间456
    end
```



#### 文件安装

[Navicat、XAMPP](https://cloud.189.cn/t/v2yU7rjQjuuq（访问码：k1yq）)：https://cloud.189.cn/t/v2yU7rjQjuuq（访问码：k1yq）

[UI](https://www.figma.com/design/vitePE5vk3yjmvhUbn1WUJ/Battle-Simulator-Game--Community-?node-id=0-1&p=f&t=wCLfdAk8gCtfEXvk-0)：[Battle Simulator Game (Community) – Figma](https://www.figma.com/design/vitePE5vk3yjmvhUbn1WUJ/Battle-Simulator-Game--Community-?node-id=0-1&p=f&t=wCLfdAk8gCtfEXvk-0)