-- ============================================================
-- Usuarios de prueba
-- ============================================================
-- Credenciales:
----------------

-- ADMINISTRADOR
-- Email: [admin@inmobiliaria.com](mailto:admin@inmobiliaria.com)
-- Password: Admin123!
----------------------

-- EMPLEADO
-- Email: [empleado@inmobiliaria.com](mailto:empleado@inmobiliaria.com)
-- Password: Empleado123!
-- ============================================================

INSERT INTO usuarios (
nombre,
apellido,
dni,
email,
avatar,
password,
rol,
activo
)
VALUES
(
'Administrador',
'Sistema',
'00000000',
'admin@inmobiliaria',
"avatar-admin",
'1ueK+uris4vRFMCLfkfRIL4UFndi3sKIxhS6bZ96qD4=',
'ADMINISTRADOR',
TRUE
),
(
'Empleado',
'Prueba',
'00000001',
'empleado@inmobiliaria.com',
"avatar-prueba",
'uIer3Li3qX3sLD6jm7iJDYQXqVeh/Ch6gxDqUsxmG1I=',
'EMPLEADO',
TRUE
);
