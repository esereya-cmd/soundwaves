
-- Soundwaves SOC 2 CC4.1
-- Database Security Control Assessment
-- MySQL 8.4
-- Read-only: does not modify database records.

-- Check 1: Confirm required security-related tables exist.
SELECT
    'Required security tables' AS control_name,
    CASE WHEN COUNT(*) = 2 THEN 'PASS' ELSE 'FAIL' END AS result
FROM information_schema.tables
WHERE table_schema = DATABASE()
  AND table_name IN ('users', 'auth_sessions');

-- Check 2: Confirm password hashes are required.
SELECT
    'Password hash column' AS control_name,
    CASE WHEN COUNT(*) = 1 THEN 'PASS' ELSE 'FAIL' END AS result
FROM information_schema.columns
WHERE table_schema = DATABASE()
  AND table_name = 'users'
  AND column_name = 'password_hash'
  AND is_nullable = 'NO';

-- Check 3: Confirm user roles are defined.
SELECT
    'User role column' AS control_name,
    CASE WHEN COUNT(*) = 1 THEN 'PASS' ELSE 'FAIL' END AS result
FROM information_schema.columns
WHERE table_schema = DATABASE()
  AND table_name = 'users'
  AND column_name = 'role'
  AND column_type = "enum('user','admin')"
  AND is_nullable = 'NO';

-- Check 4: Confirm session tokens are stored in a hash column.
SELECT
    'Session token hash column' AS control_name,
    CASE WHEN COUNT(*) = 1 THEN 'PASS' ELSE 'FAIL' END AS result
FROM information_schema.columns
WHERE table_schema = DATABASE()
  AND table_name = 'auth_sessions'
  AND column_name = 'refresh_token_hash'
  AND data_type = 'char'
  AND character_maximum_length = 64
  AND is_nullable = 'NO';

-- Check 5: Confirm refresh token hashes are unique.
SELECT
    'Unique session token hash' AS control_name,
    CASE WHEN COUNT(*) >= 1 THEN 'PASS' ELSE 'FAIL' END AS result
FROM information_schema.statistics
WHERE table_schema = DATABASE()
  AND table_name = 'auth_sessions'
  AND column_name = 'refresh_token_hash'
  AND non_unique = 0;

-- Check 6: Confirm foreign-key enforcement is enabled.
SELECT
    'Foreign key enforcement' AS control_name,
    CASE WHEN @@GLOBAL.foreign_key_checks = 1
         THEN 'PASS' ELSE 'FAIL' END AS result;

-- Check 7: Confirm password hashes are not empty.
SELECT
    'Nonempty password hashes' AS control_name,
    CASE WHEN COUNT(*) = 0 THEN 'PASS' ELSE 'FAIL' END AS result
FROM users
WHERE password_hash IS NULL
   OR TRIM(password_hash) = '';

-- Check 8: Confirm session records reference existing users.
SELECT
    'Session user relationships' AS control_name,
    CASE WHEN COUNT(*) = 0 THEN 'PASS' ELSE 'FAIL' END AS result
FROM auth_sessions s
LEFT JOIN users u ON s.user_id = u.id
WHERE u.id IS NULL;