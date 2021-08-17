# AspNet Identity

**Once you clone/download this repo. After package restore please run "Update-Database" command in package console window. So, DB can setup and application run smoothly. Also make sure redis cache has been setup. **

## Tech Stack
```
 Dot Net 5.0 (C#)
 MySql       (Database)
 redis       (Cache)
 Entity Framework (Code First)
```
## Postman
```
 Postman collection also available in this repo. I have configured env for postman. After running application you can run method in following order
 1 - Register   -- Register a user
 2 - Login      -- Login with registered user by providing detail in body. If logged in successfully API will return access token and postman will auto set in env
 3 - RegisterAppAndGetKey -- If want to register any app, run this method. It will save app into db and create client id and secret key for that app. On response  postman will set client-id, secret-key and app-name in env variable which will use for token generation
 4 - GetRegisteredApps -- Return list of registered apps
 5 - GenerateToken -- It will generate token for app and on response set app-access-token in postman env variable.
 6 - Verify -- Take app-access-token from env and verify either this is authenticated and authorized request or not.
 7 - GetPublicKey -- Return public key
```

## Controllers
```
1 - AccountController          -- Manage login and register user
2 - AppRegistrationController  -- Manage app registration and generate access token for registered apps
3 - VerificationController     -- Verify request (app to app) using client id and secret key
```
## Services
```
1 - SecurityService              -- Generate access token
2 - RSAEncryptionService         -- Encryption and decryption using public and private key
3 - CacheService                 -- Redis cache
4 - AppRegistrationService       -- App registration, save into db, client id and secret key generation
```
## Models
```
1 - ApplicationUser              -- For login and register user
2 - ApplicationRegistration      -- For app registration
3 - Response                     -- For response to client
4 - Token                        -- For token
```

