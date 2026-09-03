CREATE TABLE tipos_inmueble (
	id INT AUTO_INCREMENT PRIMARY KEY,
	nombre VARCHAR(100) NOT NULL,
	descripcion VARCHAR(255),
	activo BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE usuarios (
	id INT AUTO_INCREMENT PRIMARY KEY,
	nombre VARCHAR(100) NOT NULL,
	apellido VARCHAR(100) NOT NULL,
	email VARCHAR(254) NOT NULL UNIQUE,
	avatar VARCHAR(255),
	rol ENUM('ADMINISTRADOR', 'EMPLEADO') NOT NULL DEFAULT 'EMPLEADO',
	activo BOOLEAN NOT NULL DEFAULT TRUE
);


CREATE TABLE propietarios (
	id INT AUTO_INCREMENT PRIMARY KEY,
	nombre VARCHAR(100) NOT NULL,
	apellido VARCHAR(100) NOT NULL,
	dni VARCHAR(10) NOT NULL UNIQUE,
	email VARCHAR(254) NOT NULL UNIQUE,
	activo BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE inquilinos (
	id INT AUTO_INCREMENT PRIMARY KEY,
	nombre VARCHAR(100) NOT NULL,
	apellido VARCHAR(100) NOT NULL,
	dni VARCHAR(10) NOT NULL UNIQUE,
	email VARCHAR(254) NOT NULL UNIQUE,
	activo BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE inmuebles (
	id INT AUTO_INCREMENT PRIMARY KEY,
	propietario_id INT NOT NULL,
	tipo_inmueble_id INT NOT NULL,
	direccion VARCHAR(255) NOT NULL,
	cordenadas VARCHAR(100),
	cupo INT NOT NULL DEFAULT 1,
	precio_dia DECIMAL(10,2) NOT NULL DEFAULT 0,
	porcentaje_reserva DECIMAL(5,2) NOT NULL DEFAULT 0,
	disponible BOOLEAN NOT NULL DEFAULT TRUE,
	activo BOOLEAN NOT NULL DEFAULT TRUE,
	
	FOREIGN KEY (propietario_id) REFERENCES propietarios(id),
	FOREIGN KEY (tipo_inmueble_id) REFERENCES tipos_inmueble(id)
);

CREATE TABLE imagenes_inmueble (
	id INT AUTO_INCREMENT PRIMARY KEY,
	path VARCHAR(255) NOT NULL,
	es_portada BOOLEAN NOT NULL DEFAULT FALSE,
	inmueble_id INT NOT NULL,

	FOREIGN KEY (inmueble_id) REFERENCES inmuebles(id)
);

CREATE TABLE reservas (
	id INT AUTO_INCREMENT PRIMARY KEY,
	inmueble_id INT NOT NULL,
	inquilino_id INT NOT NULL,
	usuario_creador_id INT NOT NULL,
	usuario_cancelador_id INT NULL,
	fecha_desde DATE NOT NULL,
	fecha_hasta DATE NOT NULL,
	cancelada BOOLEAN NOT NULL DEFAULT FALSE,
	fecha_creacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	fecha_cancelacion DATETIME NULL,
	
	FOREIGN KEY (inmueble_id) REFERENCES inmuebles(id),
	FOREIGN KEY (inquilino_id) REFERENCES inquilinos(id),
	FOREIGN KEY (usuario_creador_id) REFERENCES usuarios(id),
	FOREIGN KEY (usuario_cancelador_id) REFERENCES usuarios(id)
);

CREATE TABLE pagos (
	id INT AUTO_INCREMENT PRIMARY KEY,
	reserva_id INT NOT NULL,
	usuario_creador_id INT NOT NULL,
	usuario_cancelador_id INT NULL,
	concepto VARCHAR(100),
	fecha DATE NOT NULL,
	importe DECIMAL(10,2) NOT NULL DEFAULT 0,
	anulado BOOLEAN NOT NULL DEFAULT FALSE,
	fecha_creacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	fecha_cancelacion DATETIME NULL,
	
	FOREIGN KEY (reserva_id) REFERENCES reservas(id),
	FOREIGN KEY (usuario_creador_id) REFERENCES usuarios(id),
	FOREIGN KEY (usuario_cancelador_id) REFERENCES usuarios(id)
);
