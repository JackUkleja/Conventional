﻿SELECT name, type FROM sys.key_constraints
WHERE is_system_named = 1 AND type = 'UQ'
ORDER BY name
