# AspNet Identity

**Once you clone/download this repo. After package restore please run "Update-Database" command in package console window. So, DB can setup and application run smoothly.**

## Tech Stack
```
 Dot Net 5.0 (C#)
 MySql       (Database)
 redis       (Cache)
 Entity Framework (Code First)
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

