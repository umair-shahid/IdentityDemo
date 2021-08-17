# AspNet Identity

## Controller
```
1 - AccountController          -- Manage login and register user
2 - AppRegistrationController  -- Manage app registration and generate access token for registered apps
3 - VerificationController     -- Verify request (M2M) using client id and secret key
```
## Services
```
1 - SecurityService              -- Generate access token
2 - RSAEncryptionService         -- Encryptiona and decryption using public and private key
3 - CacheService                 -- Redis cache (centerl)
4 - AppRegistrationService       -- App registration, save into db, client id and secret key generation
```
## Models
```
1 - ApplicationUser              -- For login and register user
2 - ApplicationRegistration      -- For app registration
3 - Response                     -- For response to client
4 - Token                        -- For token
```
