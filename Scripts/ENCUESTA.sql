/*
Navicat Oracle Data Transfer
Oracle Client Version : 10.2.0.5.0

Source Server         : 127.0.0.1_1521_XE
Source Server Version : 110200
Source Host           : 127.0.0.1:1521
Source Schema         : EEEE

Target Server Type    : ORACLE
Target Server Version : 110200
File Encoding         : 65001

Date: 2015-04-24 21:39:59
*/


-- ----------------------------
-- Table structure for CAT_CARGO

CREATE TABLE "CAT_CARGO" (
"id_cargo" NUMBER(11) NOT NULL ,
"nombre" VARCHAR2(50 BYTE) NOT NULL ,
"estado" NUMBER DEFAULT 0  NOT NULL ,
"creado" DATE NULL ,
"modificado" DATE NULL ,
"usuario_creacion" VARCHAR2(50 BYTE) NULL ,
"usuario_modificacion" VARCHAR2(50 BYTE) NULL 
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
-- Table structure for CAT_CIIU
-- ----------------------------

CREATE TABLE "CAT_CIIU" (
"codigo" VARCHAR2(4 BYTE) NOT NULL ,
"nombre" VARCHAR2(255 BYTE) NOT NULL ,
"estado" NUMBER DEFAULT 0 NOT NULL ,
"revision" NUMBER DEFAULT 4 NOT NULL ,
"id_ciiu" NUMBER(11) NOT NULL ,
"creado" DATE NULL ,
"modificado" DATE NULL ,
"usuario_creacion" VARCHAR2(50 BYTE) NULL ,
"usuario_modificacion" VARCHAR2(50 BYTE) NULL,
"id_metodo_calculo" NUMBER(11) NULL,
"sub_sector" NUMBER(11) DEFAULT 1 NOT NULL,
"rubro" NUMBER(11)  DEFAULT 1 NOT NULL
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
-- Table structure for CAT_CONTACTO
-- ----------------------------

CREATE TABLE "CAT_CONTACTO" (
"id_contacto" NUMBER(11) NOT NULL ,
"estado" NUMBER DEFAULT 0  NOT NULL ,
"nombre" VARCHAR2(100 BYTE) NOT NULL ,
"telefono" VARCHAR2(10 BYTE) NULL ,
"correo" VARCHAR2(50 BYTE) NOT NULL ,
"anexo" VARCHAR2(5 BYTE) NULL ,
"celular" VARCHAR2(10 BYTE) NULL ,
"id_establecimiento" NUMBER(11) NOT NULL ,
"id_cargo" NUMBER(11) NULL ,
"creado" DATE NULL ,
"modificado" DATE NULL ,
"usuario_creacion" VARCHAR2(50 BYTE) NULL ,
"usuario_modificacion" VARCHAR2(50 BYTE) NULL 
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
-- Table structure for DAT_ENCUESTA
-- ----------------------------

CREATE TABLE "DAT_ENCUESTA" (
"id_ENCUESTA" NUMBER(11) NOT NULL ,
"id_establecimiento" NUMBER(11) NOT NULL ,
"estado_ENCUESTA" NUMBER DEFAULT 0  NOT NULL ,
"fecha" DATE NOT NULL ,
"justificacion" VARCHAR2(1000 BYTE) NULL ,
"id_informante" NUMBER NULL ,
"creado" DATE NULL ,
"modificado" DATE NULL ,
"usuario_creacion" VARCHAR2(50 BYTE) NULL ,
"usuario_modificacion" VARCHAR2(50 BYTE) NULL ,
"fecha_ultimo_envio" DATE NULL 
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
-- Table structure for DAT_ENCUESTA_EMPRESARIAL
-- ----------------------------

CREATE TABLE "DAT_ENCUESTA_EMPRESARIAL" (
"id_ENCUESTA" NUMBER(11) NOT NULL 
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
-- Table structure for DAT_ENCUESTA_ESTADISTICA
-- ----------------------------

CREATE TABLE "DAT_ENCUESTA_ESTADISTICA" (
"id_ENCUESTA" NUMBER(11) NOT NULL ,
"fecha_validacion" DATE NULL,
"actualizacion" NUMBER DEFAULT 0 NOT NULL
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
-- Table structure for CAT_ESTAB_CIIU
-- ----------------------------

CREATE TABLE "CAT_ESTAB_CIIU" (
"id_estab_ciiu" NUMBER(11) NOT NULL ,
"id_establecimiento" NUMBER(11) NOT NULL ,
"id_ciiu" NUMBER(11) NOT NULL,
"creado" DATE NULL ,
"modificado" DATE NULL ,
"usuario_creacion" VARCHAR2(50 BYTE) NULL ,
"usuario_modificacion" VARCHAR2(50 BYTE) NULL 
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
-- Table structure for CAT_ESTAB_LINEA_PROD
-- ----------------------------

CREATE TABLE "CAT_ESTAB_LINEA_PROD" (
"id_establecimiento" NUMBER(11) NOT NULL ,
"id_linea_prod" NUMBER(11) NOT NULL ,
"id_estab_linea_prod" NUMBER(11) NOT NULL ,
"creado" DATE NULL ,
"modificado" DATE NULL ,
"usuario_creacion" VARCHAR2(50 BYTE) NULL ,
"usuario_modificacion" VARCHAR2(50 BYTE) NULL,
"fecha_creacion_informante" DATE NULL
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
-- Table structure for CAT_ESTABLECIMIENTO
-- ----------------------------

CREATE TABLE "CAT_ESTABLECIMIENTO" (
"id_establecimiento" NUMBER(11) NOT NULL ,
"nombre" VARCHAR2(150 BYTE) NOT NULL ,
"identificador_interno" VARCHAR2(10 BYTE) NOT NULL ,
"ubigeo" VARCHAR2(6 BYTE) NOT NULL ,
"razon_social" VARCHAR2(1000 BYTE) NULL ,
"ruc" VARCHAR2(11 BYTE) NOT NULL ,
"direccion" VARCHAR2(255 BYTE) NOT NULL ,
"telefono" VARCHAR2(10 BYTE) NOT NULL ,
"fax" VARCHAR2(100 BYTE) NULL ,
"estado" NUMBER DEFAULT 0  NOT NULL ,
"id_informante" NUMBER NULL ,
"observaciones" VARCHAR2(1000 BYTE) NULL ,
"creado" DATE NULL ,
"modificado" DATE NULL ,
"usuario_creacion" VARCHAR2(50 BYTE) NULL ,
"usuario_modificacion" VARCHAR2(50 BYTE) NULL,
"tipo_establecimiento" number default 0,
"enviar_correo" NUMBER DEFAULT 0  NOT NULL,
"ultima_notificacion" DATE NULL
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
-- Table structure for CAT_FACTOR
-- ----------------------------

CREATE TABLE "CAT_FACTOR" (
"id_factor" NUMBER(11) NOT NULL ,
"tipo_factor" NUMBER DEFAULT 0  NOT NULL ,
"nombre" VARCHAR2(150 BYTE) NOT NULL ,
"estado" NUMBER DEFAULT 0  NOT NULL ,
"creado" DATE NULL ,
"modificado" DATE NULL ,
"usuario_creacion" VARCHAR2(50 BYTE) NULL ,
"usuario_modificacion" VARCHAR2(50 BYTE) NULL 
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
-- Table structure for DAT_FACTOR_FACTOR_PROD
-- ----------------------------

CREATE TABLE "DAT_FACTOR_FACTOR_PROD" (
"id_factor" NUMBER(11) NOT NULL ,
"id_factor_produccion" NUMBER(11) NOT NULL 
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
-- Table structure for DAT_FACTOR_PRODUCCION
-- ----------------------------

CREATE TABLE "DAT_FACTOR_PRODUCCION" (
"id_factor_produccion" NUMBER(11) NOT NULL ,
"produccion_normal" NUMBER(1) DEFAULT 0  NOT NULL ,
"incremento" NUMBER(1) DEFAULT 0  NOT NULL ,
"decrecimiento" NUMBER(1) DEFAULT 1  NOT NULL ,
"creado" DATE NULL ,
"modificado" DATE NULL ,
"usuario_creacion" VARCHAR2(50 BYTE) NULL ,
"usuario_modificacion" VARCHAR2(50 BYTE) NULL,
"otro_factor" VARCHAR2(250 BYTE)  NULL 
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
-- Table structure for CAT_LINEA_PRODUCTO
-- ----------------------------

CREATE TABLE "CAT_LINEA_PRODUCTO" (
"id_linea_prod" NUMBER(11) NOT NULL ,
"nombre" VARCHAR2(250 BYTE) NOT NULL ,
"codigo" VARCHAR2(20 BYTE) NOT NULL ,
"estado" NUMBER DEFAULT 0  NOT NULL ,
"id_ciiu" NUMBER(11) NOT NULL ,
"creado" DATE NULL ,
"modificado" DATE NULL ,
"usuario_creacion" VARCHAR2(50 BYTE) NULL ,
"usuario_modificacion" VARCHAR2(50 BYTE) NULL 
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
-- Table structure for DAT_MATERIA_PROPIA
-- ----------------------------

CREATE TABLE "DAT_MATERIA_PROPIA" (
"id_materia_propia" NUMBER(11) NOT NULL ,
"id_linea_producto" NUMBER(11) NOT NULL ,
"id_unidad_medida" NUMBER(11) NOT NULL ,
"justificacion_valor_unitario" VARCHAR2(1000 BYTE) NULL ,
"justificacion_produccion" VARCHAR2(1000 BYTE) NULL ,
"valor unitario" NUMBER NULL ,
"existencia" NUMBER NULL ,
"produccion" NUMBER NULL ,
"otros_ingresos" NUMBER NULL ,
"ventas_pais" NUMBER NULL ,
"ventas_extranjeros" NUMBER NULL ,
"otras_salidas" NUMBER NULL ,
"id_volumen_produccion" NUMBER(11) NULL ,
"creado" DATE NULL ,
"modificado" DATE NULL ,
"usuario_creacion" VARCHAR2(50 BYTE) NULL ,
"usuario_modificacion" VARCHAR2(50 BYTE) NULL,
"justificacion_venta_pais" VARCHAR2(1000) NULL,
"justificacion_venta_extranjero" VARCHAR2(1000) NULL
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
-- Table structure for DAT_MATERIA_TERCEROS
-- ----------------------------

CREATE TABLE "DAT_MATERIA_TERCEROS" (
"id_materia_terceros" NUMBER(11) NOT NULL ,
"id_linea_producto" NUMBER(11) NOT NULL ,
"id_unidad_medida" NUMBER(11) NOT NULL ,
"unidad_produccion" VARCHAR2(1000 BYTE) NULL ,
"id_volumen_prod" NUMBER(11) NOT NULL ,
"creado" DATE NULL ,
"modificado" DATE NULL ,
"usuario_creacion" VARCHAR2(50 BYTE) NULL ,
"usuario_modificacion" VARCHAR2(50 BYTE) NULL,
"justificacion_produccion" VARCHAR2(1000) NULL
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
-- Table structure for CAT_POSIBLE_RESPUESTA
-- ----------------------------

CREATE TABLE "CAT_POSIBLE_RESPUESTA" (
"id_posible_respuesta" NUMBER(11) NOT NULL ,
"tipo_posible_respuesta" NUMBER DEFAULT 0  NOT NULL ,
"id_pregunta" NUMBER(11) NOT NULL,
"creado" DATE NULL ,
"modificado" DATE NULL ,
"usuario_creacion" VARCHAR2(50 BYTE) NULL ,
"usuario_modificacion" VARCHAR2(50 BYTE) NULL 
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
-- Table structure for DAT_PREGUNTA
-- ----------------------------

CREATE TABLE "DAT_PREGUNTA" (
"id_pregunta" NUMBER(11) NOT NULL ,
"texto " VARCHAR2(1000 BYTE) NOT NULL ,
"estado" NUMBER DEFAULT 0  NOT NULL ,
"comienza" DATE NULL ,
"intervalo" NUMBER(2) NULL ,
"id_encuesta_empresarial" NUMBER(11) NULL ,
"creado" DATE NULL ,
"modificado" DATE NULL ,
"usuario_creacion" VARCHAR2(50 BYTE) NULL ,
"usuario_modificacion" VARCHAR2(50 BYTE) NULL ,
"orden" NUMBER(4) DEFAULT 0  NOT NULL,
"todas_alternativas_oblig" NUMBER DEFAULT 0
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
-- Table structure for DAT_TRABAJADORES_DIAS_TRAB
-- ----------------------------

CREATE TABLE "DAT_TRABAJADORES_DIAS_TRAB" (
"id_trabajadores_dias_trabajado" NUMBER(11) NOT NULL ,
"dias_trabajados" NUMBER NULL ,
"trabajadores_produccion" NUMBER NULL ,
"administrativos_planta" NUMBER NULL ,
"creado" DATE NULL ,
"modificado" DATE NULL ,
"usuario_creacion" VARCHAR2(50 BYTE) NULL ,
"usuario_modificacion" VARCHAR2(50 BYTE) NULL 
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
-- Table structure for CAT_UNIDAD_MEDIDA
-- ----------------------------

CREATE TABLE "CAT_UNIDAD_MEDIDA" (
"descripcion" VARCHAR2(100 BYTE) NOT NULL ,
"abreviatura" VARCHAR2(5 BYTE) NOT NULL ,
"Id_unidad_medida" NUMBER(11) NOT NULL ,
"estado" NUMBER DEFAULT 0  NOT NULL ,
"creado" DATE NULL ,
"modificado" DATE NULL ,
"usuario_creacion" VARCHAR2(50 BYTE) NULL ,
"usuario_modificacion" VARCHAR2(50 BYTE) NULL 
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
-- Table structure for CAT_VALOR
-- ----------------------------

CREATE TABLE "CAT_VALOR" (
"id_valor" NUMBER(11) NOT NULL ,
"texto" VARCHAR2(1000 BYTE) NOT NULL ,
"estado" NUMBER DEFAULT 0 NOT NULL ,
"id_posible_respuesta" NUMBER(11) NOT NULL ,
"id_pregunta" NUMBER(11) NULL ,
"creado" DATE NULL ,
"modificado" DATE NULL ,
"usuario_creacion" VARCHAR2(50 BYTE) NULL ,
"usuario_modificacion" VARCHAR2(50 BYTE) NULL,
"texto_personalizado" VARCHAR2(255 BYTE)  NULL,
"es_personalizado" NUMBER DEFAULT 0  NOT NULL,
"orden" NUMBER DEFAULT 0 NULL
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
-- Table structure for DAT_VALOR_PROD_MENSUAL
-- ----------------------------

CREATE TABLE "DAT_VALOR_PROD_MENSUAL" (
"id_valor_prod" NUMBER(11) NOT NULL ,
"prod_materia_propia" NUMBER NULL ,
"prod_materia_terceros" NUMBER NULL ,
"id_ciiu" NUMBER(11) NOT NULL ,
"id_encuesta" NUMBER(11) NOT NULL,
"justificacion_materia_prop" VARCHAR2(1000) NULL,
"justificacion_materia_terc" VARCHAR2(1000) NULL
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
-- Table structure for DAT_VENTA_SERV_MANUF
-- ----------------------------

CREATE TABLE "DAT_VENTA_SERV_MANUF" (
"id_venta_servicio" NUMBER(11) NOT NULL ,
"venta" NUMBER NULL ,
"id_venta_prod_estab" NUMBER(11) NOT NULL ,
"ciiu" VARCHAR2(4 BYTE) NOT NULL ,
"detalle" VARCHAR2(255 BYTE) NOT NULL ,
"venta_extranjero" NUMBER NULL,
"justificacion_venta_pais" VARCHAR2(1000) NULL,
"justificacion_venta_ext" VARCHAR2(1000) NULL
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
-- Table structure for DAT_VENTAS_PAIS_EXTRANJERO
-- ----------------------------

CREATE TABLE "DAT_VENTAS_PAIS_EXTRANJERO" (
"id_venta_pais_extranjero" NUMBER(11) NOT NULL ,
"venta_pais" NUMBER NULL ,
"venta_extranjero" NUMBER NULL ,
"id_ciiu" NUMBER(11) NOT NULL ,
"id_ventas_producto" NUMBER(11) NOT NULL,
"justificacion_venta_pais" VARCHAR2(1000) NULL,
"justificacion_venta_ext" VARCHAR2(1000) NULL
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
-- Table structure for DAT_VENTAS_PROD_ESTAB
-- ----------------------------

CREATE TABLE "DAT_VENTAS_PROD_ESTAB" (
"id_ventas_prod" NUMBER(11) NOT NULL ,
"brindo_servicios" NUMBER(1) DEFAULT 1  NOT NULL 
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
-- Table structure for DAT_VOLUMEN_PRODUCCION
-- ----------------------------

CREATE TABLE "DAT_VOLUMEN_PRODUCCION" (
"id_volumen_prod" NUMBER(11) NOT NULL 
)
LOGGING
NOCOMPRESS
NOCACHE

;


-- ----------------------------
-- Table structure for SEG_ROL
-- ----------------------------
CREATE TABLE "SEG_ROL" (
"id_rol" NUMBER(11) NOT NULL ,
"nombre" NCLOB NOT NULL ,
"estado" NUMBER(11) NOT NULL 
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
-- Table structure for SEG_USUARIO
-- ----------------------------
CREATE TABLE "SEG_USUARIO" (
"id_usuario" NUMBER NOT NULL ,
"login" VARCHAR2(255 BYTE) NOT NULL ,
"creado" DATE NULL ,
"modificado" DATE NULL ,
"usuario_creacion" VARCHAR2(50 BYTE) NULL ,
"usuario_modificacion" VARCHAR2(50 BYTE) NULL 
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
-- Table structure for SEG_USUARIO_ROL
-- ----------------------------
CREATE TABLE "SEG_USUARIO_ROL" (
"id_usuario" NUMBER NOT NULL ,
"id_rol" NUMBER NOT NULL 
)
LOGGING
NOCOMPRESS
NOCACHE

;

-- ----------------------------
ALTER TABLE "CAT_CARGO" ADD CHECK ("id_cargo" IS NOT NULL);
ALTER TABLE "CAT_CARGO" ADD CHECK ("nombre" IS NOT NULL);
ALTER TABLE "CAT_CARGO" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "CAT_CARGO" ADD CHECK ("id_cargo" IS NOT NULL);
ALTER TABLE "CAT_CARGO" ADD CHECK ("nombre" IS NOT NULL);
ALTER TABLE "CAT_CARGO" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "CAT_CARGO" ADD CHECK ("id_cargo" IS NOT NULL);
ALTER TABLE "CAT_CARGO" ADD CHECK ("nombre" IS NOT NULL);
ALTER TABLE "CAT_CARGO" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "CAT_CARGO" ADD CHECK ("id_cargo" IS NOT NULL);
ALTER TABLE "CAT_CARGO" ADD CHECK ("nombre" IS NOT NULL);
ALTER TABLE "CAT_CARGO" ADD CHECK ("estado" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table CAT_CARGO
-- ----------------------------
ALTER TABLE "CAT_CARGO" ADD PRIMARY KEY ("id_cargo");

-- ----------------------------
-- Indexes structure for table CAT_CIIU
-- ----------------------------

-- ----------------------------
-- Checks structure for table CAT_CIIU
-- ----------------------------
ALTER TABLE "CAT_CIIU" ADD CHECK ("codigo" IS NOT NULL);
ALTER TABLE "CAT_CIIU" ADD CHECK ("nombre" IS NOT NULL);
ALTER TABLE "CAT_CIIU" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "CAT_CIIU" ADD CHECK ("id_ciiu" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table CAT_CIIU
-- ----------------------------
ALTER TABLE "CAT_CIIU" ADD PRIMARY KEY ("id_ciiu");

-- ----------------------------
-- Indexes structure for table CAT_CONTACTO
-- ----------------------------

-- ----------------------------
-- Checks structure for table CAT_CONTACTO
-- ----------------------------
ALTER TABLE "CAT_CONTACTO" ADD CHECK ("id_contacto" IS NOT NULL);
ALTER TABLE "CAT_CONTACTO" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "CAT_CONTACTO" ADD CHECK ("nombre" IS NOT NULL);
ALTER TABLE "CAT_CONTACTO" ADD CHECK ("correo" IS NOT NULL);
ALTER TABLE "CAT_CONTACTO" ADD CHECK ("id_establecimiento" IS NOT NULL);
ALTER TABLE "CAT_CONTACTO" ADD CHECK ("id_contacto" IS NOT NULL);
ALTER TABLE "CAT_CONTACTO" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "CAT_CONTACTO" ADD CHECK ("nombre" IS NOT NULL);
ALTER TABLE "CAT_CONTACTO" ADD CHECK ("correo" IS NOT NULL);
ALTER TABLE "CAT_CONTACTO" ADD CHECK ("id_establecimiento" IS NOT NULL);
ALTER TABLE "CAT_CONTACTO" ADD CHECK ("id_contacto" IS NOT NULL);
ALTER TABLE "CAT_CONTACTO" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "CAT_CONTACTO" ADD CHECK ("nombre" IS NOT NULL);
ALTER TABLE "CAT_CONTACTO" ADD CHECK ("correo" IS NOT NULL);
ALTER TABLE "CAT_CONTACTO" ADD CHECK ("id_establecimiento" IS NOT NULL);
ALTER TABLE "CAT_CONTACTO" ADD CHECK ("id_contacto" IS NOT NULL);
ALTER TABLE "CAT_CONTACTO" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "CAT_CONTACTO" ADD CHECK ("nombre" IS NOT NULL);
ALTER TABLE "CAT_CONTACTO" ADD CHECK ("correo" IS NOT NULL);
ALTER TABLE "CAT_CONTACTO" ADD CHECK ("id_establecimiento" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table CAT_CONTACTO
-- ----------------------------
ALTER TABLE "CAT_CONTACTO" ADD PRIMARY KEY ("id_contacto");

-- ----------------------------
-- Indexes structure for table DAT_ENCUESTA
-- ----------------------------

-- ----------------------------
-- Checks structure for table DAT_ENCUESTA
-- ----------------------------
ALTER TABLE "DAT_ENCUESTA" ADD CHECK ("id_ENCUESTA" IS NOT NULL);
ALTER TABLE "DAT_ENCUESTA" ADD CHECK ("id_establecimiento" IS NOT NULL);
ALTER TABLE "DAT_ENCUESTA" ADD CHECK ("estado_ENCUESTA" IS NOT NULL);
ALTER TABLE "DAT_ENCUESTA" ADD CHECK ("fecha" IS NOT NULL);
ALTER TABLE "DAT_ENCUESTA" ADD CHECK ("id_ENCUESTA" IS NOT NULL);
ALTER TABLE "DAT_ENCUESTA" ADD CHECK ("id_establecimiento" IS NOT NULL);
ALTER TABLE "DAT_ENCUESTA" ADD CHECK ("estado_ENCUESTA" IS NOT NULL);
ALTER TABLE "DAT_ENCUESTA" ADD CHECK ("fecha" IS NOT NULL);
ALTER TABLE "DAT_ENCUESTA" ADD CHECK ("id_ENCUESTA" IS NOT NULL);
ALTER TABLE "DAT_ENCUESTA" ADD CHECK ("id_establecimiento" IS NOT NULL);
ALTER TABLE "DAT_ENCUESTA" ADD CHECK ("estado_ENCUESTA" IS NOT NULL);
ALTER TABLE "DAT_ENCUESTA" ADD CHECK ("fecha" IS NOT NULL);
ALTER TABLE "DAT_ENCUESTA" ADD CHECK ("id_ENCUESTA" IS NOT NULL);
ALTER TABLE "DAT_ENCUESTA" ADD CHECK ("id_establecimiento" IS NOT NULL);
ALTER TABLE "DAT_ENCUESTA" ADD CHECK ("estado_ENCUESTA" IS NOT NULL);
ALTER TABLE "DAT_ENCUESTA" ADD CHECK ("fecha" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table DAT_ENCUESTA
-- ----------------------------
ALTER TABLE "DAT_ENCUESTA" ADD PRIMARY KEY ("id_ENCUESTA");

-- ----------------------------
-- Indexes structure for table DAT_ENCUESTA_EMPRESARIAL
-- ----------------------------

-- ----------------------------
-- Checks structure for table DAT_ENCUESTA_EMPRESARIAL
-- ----------------------------
ALTER TABLE "DAT_ENCUESTA_EMPRESARIAL" ADD CHECK ("id_ENCUESTA" IS NOT NULL);
ALTER TABLE "DAT_ENCUESTA_EMPRESARIAL" ADD CHECK ("id_ENCUESTA" IS NOT NULL);
ALTER TABLE "DAT_ENCUESTA_EMPRESARIAL" ADD CHECK ("id_ENCUESTA" IS NOT NULL);
ALTER TABLE "DAT_ENCUESTA_EMPRESARIAL" ADD CHECK ("id_ENCUESTA" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table DAT_ENCUESTA_EMPRESARIAL
-- ----------------------------
ALTER TABLE "DAT_ENCUESTA_EMPRESARIAL" ADD PRIMARY KEY ("id_ENCUESTA");

-- ----------------------------
-- Indexes structure for table DAT_ENCUESTA_ESTADISTICA
-- ----------------------------

-- ----------------------------
-- Checks structure for table DAT_ENCUESTA_ESTADISTICA
-- ----------------------------
ALTER TABLE "DAT_ENCUESTA_ESTADISTICA" ADD CHECK ("id_ENCUESTA" IS NOT NULL);
ALTER TABLE "DAT_ENCUESTA_ESTADISTICA" ADD CHECK ("id_ENCUESTA" IS NOT NULL);
ALTER TABLE "DAT_ENCUESTA_ESTADISTICA" ADD CHECK ("id_ENCUESTA" IS NOT NULL);
ALTER TABLE "DAT_ENCUESTA_ESTADISTICA" ADD CHECK ("id_ENCUESTA" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table DAT_ENCUESTA_ESTADISTICA
-- ----------------------------
ALTER TABLE "DAT_ENCUESTA_ESTADISTICA" ADD PRIMARY KEY ("id_ENCUESTA");

-- ----------------------------
-- Indexes structure for table CAT_ESTAB_CIIU
-- ----------------------------

-- ----------------------------
-- Checks structure for table CAT_ESTAB_CIIU
-- ----------------------------
ALTER TABLE "CAT_ESTAB_CIIU" ADD CHECK ("id_establecimiento" IS NOT NULL);
ALTER TABLE "CAT_ESTAB_CIIU" ADD CHECK ("id_ciiu" IS NOT NULL);
ALTER TABLE "CAT_ESTAB_CIIU" ADD CHECK ("id_estab_ciiu" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table CAT_ESTAB_CIIU
-- ----------------------------
ALTER TABLE "CAT_ESTAB_CIIU" ADD PRIMARY KEY ("id_estab_ciiu");

-- ----------------------------
-- Indexes structure for table CAT_ESTAB_LINEA_PROD
-- ----------------------------

-- ----------------------------
-- Checks structure for table CAT_ESTAB_LINEA_PROD
-- ----------------------------
ALTER TABLE "CAT_ESTAB_LINEA_PROD" ADD CHECK ("id_establecimiento" IS NOT NULL);
ALTER TABLE "CAT_ESTAB_LINEA_PROD" ADD CHECK ("id_linea_prod" IS NOT NULL);
ALTER TABLE "CAT_ESTAB_LINEA_PROD" ADD CHECK ("id_estab_linea_prod" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table CAT_ESTAB_LINEA_PROD
-- ----------------------------
ALTER TABLE "CAT_ESTAB_LINEA_PROD" ADD PRIMARY KEY ("id_estab_linea_prod");

-- ----------------------------
-- Indexes structure for table CAT_ESTABLECIMIENTO
-- ----------------------------

-- ----------------------------
-- Checks structure for table CAT_ESTABLECIMIENTO
-- ----------------------------
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("id_establecimiento" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("nombre" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("identificador_interno" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("ubigeo" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("ruc" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("direccion" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("telefono" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("id_establecimiento" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("nombre" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("identificador_interno" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("ubigeo" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("ruc" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("direccion" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("telefono" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("id_establecimiento" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("nombre" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("identificador_interno" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("ubigeo" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("ruc" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("direccion" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("telefono" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("id_establecimiento" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("nombre" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("identificador_interno" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("ubigeo" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("ruc" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("direccion" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("telefono" IS NOT NULL);
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD CHECK ("estado" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table CAT_ESTABLECIMIENTO
-- ----------------------------
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD PRIMARY KEY ("id_establecimiento");

-- ----------------------------
-- Indexes structure for table CAT_FACTOR
-- ----------------------------

-- ----------------------------
-- Checks structure for table CAT_FACTOR
-- ----------------------------
ALTER TABLE "CAT_FACTOR" ADD CHECK ("id_factor" IS NOT NULL);
ALTER TABLE "CAT_FACTOR" ADD CHECK ("tipo_factor" IS NOT NULL);
ALTER TABLE "CAT_FACTOR" ADD CHECK ("nombre" IS NOT NULL);
ALTER TABLE "CAT_FACTOR" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "CAT_FACTOR" ADD CHECK ("id_factor" IS NOT NULL);
ALTER TABLE "CAT_FACTOR" ADD CHECK ("tipo_factor" IS NOT NULL);
ALTER TABLE "CAT_FACTOR" ADD CHECK ("nombre" IS NOT NULL);
ALTER TABLE "CAT_FACTOR" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "CAT_FACTOR" ADD CHECK ("id_factor" IS NOT NULL);
ALTER TABLE "CAT_FACTOR" ADD CHECK ("tipo_factor" IS NOT NULL);
ALTER TABLE "CAT_FACTOR" ADD CHECK ("nombre" IS NOT NULL);
ALTER TABLE "CAT_FACTOR" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "CAT_FACTOR" ADD CHECK ("id_factor" IS NOT NULL);
ALTER TABLE "CAT_FACTOR" ADD CHECK ("tipo_factor" IS NOT NULL);
ALTER TABLE "CAT_FACTOR" ADD CHECK ("nombre" IS NOT NULL);
ALTER TABLE "CAT_FACTOR" ADD CHECK ("estado" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table CAT_FACTOR
-- ----------------------------
ALTER TABLE "CAT_FACTOR" ADD PRIMARY KEY ("id_factor");

-- ----------------------------
-- Indexes structure for table DAT_FACTOR_FACTOR_PROD
-- ----------------------------

-- ----------------------------
-- Checks structure for table DAT_FACTOR_FACTOR_PROD
-- ----------------------------
ALTER TABLE "DAT_FACTOR_FACTOR_PROD" ADD CHECK ("id_factor" IS NOT NULL);
ALTER TABLE "DAT_FACTOR_FACTOR_PROD" ADD CHECK ("id_factor_produccion" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table DAT_FACTOR_FACTOR_PROD
-- ----------------------------
ALTER TABLE "DAT_FACTOR_FACTOR_PROD" ADD PRIMARY KEY ("id_factor", "id_factor_produccion");

-- ----------------------------
-- Indexes structure for table DAT_FACTOR_PRODUCCION
-- ----------------------------

-- ----------------------------
-- Checks structure for table DAT_FACTOR_PRODUCCION
-- ----------------------------
ALTER TABLE "DAT_FACTOR_PRODUCCION" ADD CHECK ("id_factor_produccion" IS NOT NULL);
ALTER TABLE "DAT_FACTOR_PRODUCCION" ADD CHECK ("produccion_normal" IS NOT NULL);
ALTER TABLE "DAT_FACTOR_PRODUCCION" ADD CHECK ("incremento" IS NOT NULL);
ALTER TABLE "DAT_FACTOR_PRODUCCION" ADD CHECK ("decrecimiento" IS NOT NULL);
ALTER TABLE "DAT_FACTOR_PRODUCCION" ADD CHECK ("id_factor_produccion" IS NOT NULL);
ALTER TABLE "DAT_FACTOR_PRODUCCION" ADD CHECK ("produccion_normal" IS NOT NULL);
ALTER TABLE "DAT_FACTOR_PRODUCCION" ADD CHECK ("incremento" IS NOT NULL);
ALTER TABLE "DAT_FACTOR_PRODUCCION" ADD CHECK ("decrecimiento" IS NOT NULL);
ALTER TABLE "DAT_FACTOR_PRODUCCION" ADD CHECK ("id_factor_produccion" IS NOT NULL);
ALTER TABLE "DAT_FACTOR_PRODUCCION" ADD CHECK ("produccion_normal" IS NOT NULL);
ALTER TABLE "DAT_FACTOR_PRODUCCION" ADD CHECK ("incremento" IS NOT NULL);
ALTER TABLE "DAT_FACTOR_PRODUCCION" ADD CHECK ("decrecimiento" IS NOT NULL);
ALTER TABLE "DAT_FACTOR_PRODUCCION" ADD CHECK ("id_factor_produccion" IS NOT NULL);
ALTER TABLE "DAT_FACTOR_PRODUCCION" ADD CHECK ("produccion_normal" IS NOT NULL);
ALTER TABLE "DAT_FACTOR_PRODUCCION" ADD CHECK ("incremento" IS NOT NULL);
ALTER TABLE "DAT_FACTOR_PRODUCCION" ADD CHECK ("decrecimiento" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table DAT_FACTOR_PRODUCCION
-- ----------------------------
ALTER TABLE "DAT_FACTOR_PRODUCCION" ADD PRIMARY KEY ("id_factor_produccion");

-- ----------------------------
-- Indexes structure for table CAT_LINEA_PRODUCTO
-- ----------------------------

-- ----------------------------
-- Checks structure for table CAT_LINEA_PRODUCTO
-- ----------------------------
ALTER TABLE "CAT_LINEA_PRODUCTO" ADD CHECK ("id_linea_prod" IS NOT NULL);
ALTER TABLE "CAT_LINEA_PRODUCTO" ADD CHECK ("nombre" IS NOT NULL);
ALTER TABLE "CAT_LINEA_PRODUCTO" ADD CHECK ("codigo" IS NOT NULL);
ALTER TABLE "CAT_LINEA_PRODUCTO" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "CAT_LINEA_PRODUCTO" ADD CHECK ("id_ciiu" IS NOT NULL);
ALTER TABLE "CAT_LINEA_PRODUCTO" ADD CHECK ("id_linea_prod" IS NOT NULL);
ALTER TABLE "CAT_LINEA_PRODUCTO" ADD CHECK ("nombre" IS NOT NULL);
ALTER TABLE "CAT_LINEA_PRODUCTO" ADD CHECK ("codigo" IS NOT NULL);
ALTER TABLE "CAT_LINEA_PRODUCTO" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "CAT_LINEA_PRODUCTO" ADD CHECK ("id_ciiu" IS NOT NULL);
ALTER TABLE "CAT_LINEA_PRODUCTO" ADD CHECK ("id_linea_prod" IS NOT NULL);
ALTER TABLE "CAT_LINEA_PRODUCTO" ADD CHECK ("nombre" IS NOT NULL);
ALTER TABLE "CAT_LINEA_PRODUCTO" ADD CHECK ("codigo" IS NOT NULL);
ALTER TABLE "CAT_LINEA_PRODUCTO" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "CAT_LINEA_PRODUCTO" ADD CHECK ("id_ciiu" IS NOT NULL);
ALTER TABLE "CAT_LINEA_PRODUCTO" ADD CHECK ("id_linea_prod" IS NOT NULL);
ALTER TABLE "CAT_LINEA_PRODUCTO" ADD CHECK ("nombre" IS NOT NULL);
ALTER TABLE "CAT_LINEA_PRODUCTO" ADD CHECK ("codigo" IS NOT NULL);
ALTER TABLE "CAT_LINEA_PRODUCTO" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "CAT_LINEA_PRODUCTO" ADD CHECK ("id_ciiu" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table CAT_LINEA_PRODUCTO
-- ----------------------------
ALTER TABLE "CAT_LINEA_PRODUCTO" ADD PRIMARY KEY ("id_linea_prod");

-- ----------------------------
-- Indexes structure for table DAT_MATERIA_PROPIA
-- ----------------------------

-- ----------------------------
-- Checks structure for table DAT_MATERIA_PROPIA
-- ----------------------------
ALTER TABLE "DAT_MATERIA_PROPIA" ADD CHECK ("id_materia_propia" IS NOT NULL);
ALTER TABLE "DAT_MATERIA_PROPIA" ADD CHECK ("id_linea_producto" IS NOT NULL);
ALTER TABLE "DAT_MATERIA_PROPIA" ADD CHECK ("id_unidad_medida" IS NOT NULL);
ALTER TABLE "DAT_MATERIA_PROPIA" ADD CHECK ("id_materia_propia" IS NOT NULL);
ALTER TABLE "DAT_MATERIA_PROPIA" ADD CHECK ("id_linea_producto" IS NOT NULL);
ALTER TABLE "DAT_MATERIA_PROPIA" ADD CHECK ("id_unidad_medida" IS NOT NULL);
ALTER TABLE "DAT_MATERIA_PROPIA" ADD CHECK ("id_materia_propia" IS NOT NULL);
ALTER TABLE "DAT_MATERIA_PROPIA" ADD CHECK ("id_linea_producto" IS NOT NULL);
ALTER TABLE "DAT_MATERIA_PROPIA" ADD CHECK ("id_unidad_medida" IS NOT NULL);
ALTER TABLE "DAT_MATERIA_PROPIA" ADD CHECK ("id_materia_propia" IS NOT NULL);
ALTER TABLE "DAT_MATERIA_PROPIA" ADD CHECK ("id_linea_producto" IS NOT NULL);
ALTER TABLE "DAT_MATERIA_PROPIA" ADD CHECK ("id_unidad_medida" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table DAT_MATERIA_PROPIA
-- ----------------------------
ALTER TABLE "DAT_MATERIA_PROPIA" ADD PRIMARY KEY ("id_materia_propia");

-- ----------------------------
-- Indexes structure for table DAT_MATERIA_TERCEROS
-- ----------------------------

-- ----------------------------
-- Checks structure for table DAT_MATERIA_TERCEROS
-- ----------------------------
ALTER TABLE "DAT_MATERIA_TERCEROS" ADD CHECK ("id_materia_terceros" IS NOT NULL);
ALTER TABLE "DAT_MATERIA_TERCEROS" ADD CHECK ("id_linea_producto" IS NOT NULL);
ALTER TABLE "DAT_MATERIA_TERCEROS" ADD CHECK ("id_unidad_medida" IS NOT NULL);
ALTER TABLE "DAT_MATERIA_TERCEROS" ADD CHECK ("id_volumen_prod" IS NOT NULL);
ALTER TABLE "DAT_MATERIA_TERCEROS" ADD CHECK ("id_materia_terceros" IS NOT NULL);
ALTER TABLE "DAT_MATERIA_TERCEROS" ADD CHECK ("id_linea_producto" IS NOT NULL);
ALTER TABLE "DAT_MATERIA_TERCEROS" ADD CHECK ("id_unidad_medida" IS NOT NULL);
ALTER TABLE "DAT_MATERIA_TERCEROS" ADD CHECK ("id_volumen_prod" IS NOT NULL);
ALTER TABLE "DAT_MATERIA_TERCEROS" ADD CHECK ("id_materia_terceros" IS NOT NULL);
ALTER TABLE "DAT_MATERIA_TERCEROS" ADD CHECK ("id_linea_producto" IS NOT NULL);
ALTER TABLE "DAT_MATERIA_TERCEROS" ADD CHECK ("id_unidad_medida" IS NOT NULL);
ALTER TABLE "DAT_MATERIA_TERCEROS" ADD CHECK ("id_volumen_prod" IS NOT NULL);
ALTER TABLE "DAT_MATERIA_TERCEROS" ADD CHECK ("id_materia_terceros" IS NOT NULL);
ALTER TABLE "DAT_MATERIA_TERCEROS" ADD CHECK ("id_linea_producto" IS NOT NULL);
ALTER TABLE "DAT_MATERIA_TERCEROS" ADD CHECK ("id_unidad_medida" IS NOT NULL);
ALTER TABLE "DAT_MATERIA_TERCEROS" ADD CHECK ("id_volumen_prod" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table DAT_MATERIA_TERCEROS
-- ----------------------------
ALTER TABLE "DAT_MATERIA_TERCEROS" ADD PRIMARY KEY ("id_materia_terceros");

-- ----------------------------
-- Indexes structure for table CAT_POSIBLE_RESPUESTA
-- ----------------------------

-- ----------------------------
-- Checks structure for table CAT_POSIBLE_RESPUESTA
-- ----------------------------
ALTER TABLE "CAT_POSIBLE_RESPUESTA" ADD CHECK ("id_posible_respuesta" IS NOT NULL);
ALTER TABLE "CAT_POSIBLE_RESPUESTA" ADD CHECK ("tipo_posible_respuesta" IS NOT NULL);
ALTER TABLE "CAT_POSIBLE_RESPUESTA" ADD CHECK ("id_pregunta" IS NOT NULL);
ALTER TABLE "CAT_POSIBLE_RESPUESTA" ADD CHECK ("id_posible_respuesta" IS NOT NULL);
ALTER TABLE "CAT_POSIBLE_RESPUESTA" ADD CHECK ("tipo_posible_respuesta" IS NOT NULL);
ALTER TABLE "CAT_POSIBLE_RESPUESTA" ADD CHECK ("id_pregunta" IS NOT NULL);
ALTER TABLE "CAT_POSIBLE_RESPUESTA" ADD CHECK ("id_posible_respuesta" IS NOT NULL);
ALTER TABLE "CAT_POSIBLE_RESPUESTA" ADD CHECK ("tipo_posible_respuesta" IS NOT NULL);
ALTER TABLE "CAT_POSIBLE_RESPUESTA" ADD CHECK ("id_pregunta" IS NOT NULL);
ALTER TABLE "CAT_POSIBLE_RESPUESTA" ADD CHECK ("id_posible_respuesta" IS NOT NULL);
ALTER TABLE "CAT_POSIBLE_RESPUESTA" ADD CHECK ("tipo_posible_respuesta" IS NOT NULL);
ALTER TABLE "CAT_POSIBLE_RESPUESTA" ADD CHECK ("id_pregunta" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table CAT_POSIBLE_RESPUESTA
-- ----------------------------
ALTER TABLE "CAT_POSIBLE_RESPUESTA" ADD PRIMARY KEY ("id_posible_respuesta");

-- ----------------------------
-- Indexes structure for table DAT_PREGUNTA
-- ----------------------------

-- ----------------------------
-- Checks structure for table DAT_PREGUNTA
-- ----------------------------
ALTER TABLE "DAT_PREGUNTA" ADD CHECK ("id_pregunta" IS NOT NULL);
ALTER TABLE "DAT_PREGUNTA" ADD CHECK ("texto " IS NOT NULL);
ALTER TABLE "DAT_PREGUNTA" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "DAT_PREGUNTA" ADD CHECK ("id_pregunta" IS NOT NULL);
ALTER TABLE "DAT_PREGUNTA" ADD CHECK ("texto " IS NOT NULL);
ALTER TABLE "DAT_PREGUNTA" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "DAT_PREGUNTA" ADD CHECK ("id_pregunta" IS NOT NULL);
ALTER TABLE "DAT_PREGUNTA" ADD CHECK ("texto " IS NOT NULL);
ALTER TABLE "DAT_PREGUNTA" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "DAT_PREGUNTA" ADD CHECK ("id_pregunta" IS NOT NULL);
ALTER TABLE "DAT_PREGUNTA" ADD CHECK ("texto " IS NOT NULL);
ALTER TABLE "DAT_PREGUNTA" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "DAT_PREGUNTA" ADD CHECK ("orden" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table DAT_PREGUNTA
-- ----------------------------
ALTER TABLE "DAT_PREGUNTA" ADD PRIMARY KEY ("id_pregunta");

-- ----------------------------
-- Indexes structure for table DAT_TRABAJADORES_DIAS_TRAB
-- ----------------------------

-- ----------------------------
-- Checks structure for table DAT_TRABAJADORES_DIAS_TRAB
-- ----------------------------
ALTER TABLE "DAT_TRABAJADORES_DIAS_TRAB" ADD CHECK ("id_trabajadores_dias_trabajado" IS NOT NULL);
ALTER TABLE "DAT_TRABAJADORES_DIAS_TRAB" ADD CHECK ("id_trabajadores_dias_trabajado" IS NOT NULL);
ALTER TABLE "DAT_TRABAJADORES_DIAS_TRAB" ADD CHECK ("id_trabajadores_dias_trabajado" IS NOT NULL);
ALTER TABLE "DAT_TRABAJADORES_DIAS_TRAB" ADD CHECK ("id_trabajadores_dias_trabajado" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table DAT_TRABAJADORES_DIAS_TRAB
-- ----------------------------
ALTER TABLE "DAT_TRABAJADORES_DIAS_TRAB" ADD PRIMARY KEY ("id_trabajadores_dias_trabajado");

-- ----------------------------
-- Indexes structure for table CAT_UNIDAD_MEDIDA
-- ----------------------------

-- ----------------------------
-- Checks structure for table CAT_UNIDAD_MEDIDA
-- ----------------------------
ALTER TABLE "CAT_UNIDAD_MEDIDA" ADD CHECK ("descripcion" IS NOT NULL);
ALTER TABLE "CAT_UNIDAD_MEDIDA" ADD CHECK ("abreviatura" IS NOT NULL);
ALTER TABLE "CAT_UNIDAD_MEDIDA" ADD CHECK ("Id_unidad_medida" IS NOT NULL);
ALTER TABLE "CAT_UNIDAD_MEDIDA" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "CAT_UNIDAD_MEDIDA" ADD CHECK ("descripcion" IS NOT NULL);
ALTER TABLE "CAT_UNIDAD_MEDIDA" ADD CHECK ("abreviatura" IS NOT NULL);
ALTER TABLE "CAT_UNIDAD_MEDIDA" ADD CHECK ("Id_unidad_medida" IS NOT NULL);
ALTER TABLE "CAT_UNIDAD_MEDIDA" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "CAT_UNIDAD_MEDIDA" ADD CHECK ("descripcion" IS NOT NULL);
ALTER TABLE "CAT_UNIDAD_MEDIDA" ADD CHECK ("abreviatura" IS NOT NULL);
ALTER TABLE "CAT_UNIDAD_MEDIDA" ADD CHECK ("Id_unidad_medida" IS NOT NULL);
ALTER TABLE "CAT_UNIDAD_MEDIDA" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "CAT_UNIDAD_MEDIDA" ADD CHECK ("descripcion" IS NOT NULL);
ALTER TABLE "CAT_UNIDAD_MEDIDA" ADD CHECK ("abreviatura" IS NOT NULL);
ALTER TABLE "CAT_UNIDAD_MEDIDA" ADD CHECK ("Id_unidad_medida" IS NOT NULL);
ALTER TABLE "CAT_UNIDAD_MEDIDA" ADD CHECK ("estado" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table CAT_UNIDAD_MEDIDA
-- ----------------------------
ALTER TABLE "CAT_UNIDAD_MEDIDA" ADD PRIMARY KEY ("Id_unidad_medida");

-- ----------------------------
-- Indexes structure for table CAT_VALOR
-- ----------------------------

-- ----------------------------
-- Checks structure for table CAT_VALOR
-- ----------------------------
ALTER TABLE "CAT_VALOR" ADD CHECK ("id_valor" IS NOT NULL);
ALTER TABLE "CAT_VALOR" ADD CHECK ("texto" IS NOT NULL);
ALTER TABLE "CAT_VALOR" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "CAT_VALOR" ADD CHECK ("id_posible_respuesta" IS NOT NULL);
ALTER TABLE "CAT_VALOR" ADD CHECK ("id_valor" IS NOT NULL);
ALTER TABLE "CAT_VALOR" ADD CHECK ("texto" IS NOT NULL);
ALTER TABLE "CAT_VALOR" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "CAT_VALOR" ADD CHECK ("id_posible_respuesta" IS NOT NULL);
ALTER TABLE "CAT_VALOR" ADD CHECK ("id_valor" IS NOT NULL);
ALTER TABLE "CAT_VALOR" ADD CHECK ("texto" IS NOT NULL);
ALTER TABLE "CAT_VALOR" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "CAT_VALOR" ADD CHECK ("id_posible_respuesta" IS NOT NULL);
ALTER TABLE "CAT_VALOR" ADD CHECK ("id_valor" IS NOT NULL);
ALTER TABLE "CAT_VALOR" ADD CHECK ("texto" IS NOT NULL);
ALTER TABLE "CAT_VALOR" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "CAT_VALOR" ADD CHECK ("id_posible_respuesta" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table CAT_VALOR
-- ----------------------------
ALTER TABLE "CAT_VALOR" ADD PRIMARY KEY ("id_valor");

-- ----------------------------
-- Indexes structure for table DAT_VALOR_PROD_MENSUAL
-- ----------------------------

-- ----------------------------
-- Checks structure for table DAT_VALOR_PROD_MENSUAL
-- ----------------------------
ALTER TABLE "DAT_VALOR_PROD_MENSUAL" ADD CHECK ("id_valor_prod" IS NOT NULL);
ALTER TABLE "DAT_VALOR_PROD_MENSUAL" ADD CHECK ("id_valor_prod" IS NOT NULL);
ALTER TABLE "DAT_VALOR_PROD_MENSUAL" ADD CHECK ("id_valor_prod" IS NOT NULL);
ALTER TABLE "DAT_VALOR_PROD_MENSUAL" ADD CHECK ("id_valor_prod" IS NOT NULL);
ALTER TABLE "DAT_VALOR_PROD_MENSUAL" ADD CHECK ("id_ciiu" IS NOT NULL);
ALTER TABLE "DAT_VALOR_PROD_MENSUAL" ADD CHECK ("id_encuesta" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table DAT_VALOR_PROD_MENSUAL
-- ----------------------------
ALTER TABLE "DAT_VALOR_PROD_MENSUAL" ADD PRIMARY KEY ("id_valor_prod");

-- ----------------------------
-- Indexes structure for table DAT_VENTA_SERV_MANUF
-- ----------------------------

-- ----------------------------
-- Checks structure for table DAT_VENTA_SERV_MANUF
-- ----------------------------
ALTER TABLE "DAT_VENTA_SERV_MANUF" ADD CHECK ("id_venta_servicio" IS NOT NULL);
ALTER TABLE "DAT_VENTA_SERV_MANUF" ADD CHECK ("id_venta_prod_estab" IS NOT NULL);
ALTER TABLE "DAT_VENTA_SERV_MANUF" ADD CHECK ("id_venta_servicio" IS NOT NULL);
ALTER TABLE "DAT_VENTA_SERV_MANUF" ADD CHECK ("id_venta_prod_estab" IS NOT NULL);
ALTER TABLE "DAT_VENTA_SERV_MANUF" ADD CHECK ("id_venta_servicio" IS NOT NULL);
ALTER TABLE "DAT_VENTA_SERV_MANUF" ADD CHECK ("id_venta_prod_estab" IS NOT NULL);
ALTER TABLE "DAT_VENTA_SERV_MANUF" ADD CHECK ("id_venta_servicio" IS NOT NULL);
ALTER TABLE "DAT_VENTA_SERV_MANUF" ADD CHECK ("id_venta_prod_estab" IS NOT NULL);
ALTER TABLE "DAT_VENTA_SERV_MANUF" ADD CHECK ("ciiu" IS NOT NULL);
ALTER TABLE "DAT_VENTA_SERV_MANUF" ADD CHECK ("detalle" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table DAT_VENTA_SERV_MANUF
-- ----------------------------
ALTER TABLE "DAT_VENTA_SERV_MANUF" ADD PRIMARY KEY ("id_venta_servicio");

-- ----------------------------
-- Indexes structure for table DAT_VENTAS_PAIS_EXTRANJERO
-- ----------------------------

-- ----------------------------
-- Checks structure for table DAT_VENTAS_PAIS_EXTRANJERO
-- ----------------------------
ALTER TABLE "DAT_VENTAS_PAIS_EXTRANJERO" ADD CHECK ("id_venta_pais_extranjero" IS NOT NULL);
ALTER TABLE "DAT_VENTAS_PAIS_EXTRANJERO" ADD CHECK ("id_venta_pais_extranjero" IS NOT NULL);
ALTER TABLE "DAT_VENTAS_PAIS_EXTRANJERO" ADD CHECK ("id_venta_pais_extranjero" IS NOT NULL);
ALTER TABLE "DAT_VENTAS_PAIS_EXTRANJERO" ADD CHECK ("id_venta_pais_extranjero" IS NOT NULL);
ALTER TABLE "DAT_VENTAS_PAIS_EXTRANJERO" ADD CHECK ("id_ciiu" IS NOT NULL);
ALTER TABLE "DAT_VENTAS_PAIS_EXTRANJERO" ADD CHECK ("id_ventas_producto" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table DAT_VENTAS_PAIS_EXTRANJERO
-- ----------------------------
ALTER TABLE "DAT_VENTAS_PAIS_EXTRANJERO" ADD PRIMARY KEY ("id_venta_pais_extranjero");

-- ----------------------------
-- Indexes structure for table DAT_VENTAS_PROD_ESTAB
-- ----------------------------

-- ----------------------------
-- Checks structure for table DAT_VENTAS_PROD_ESTAB
-- ----------------------------
ALTER TABLE "DAT_VENTAS_PROD_ESTAB" ADD CHECK ("id_ventas_prod" IS NOT NULL);
ALTER TABLE "DAT_VENTAS_PROD_ESTAB" ADD CHECK ("id_ventas_prod" IS NOT NULL);
ALTER TABLE "DAT_VENTAS_PROD_ESTAB" ADD CHECK ("id_ventas_prod" IS NOT NULL);
ALTER TABLE "DAT_VENTAS_PROD_ESTAB" ADD CHECK ("id_ventas_prod" IS NOT NULL);
ALTER TABLE "DAT_VENTAS_PROD_ESTAB" ADD CHECK ("brindo_servicios" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table DAT_VENTAS_PROD_ESTAB
-- ----------------------------
ALTER TABLE "DAT_VENTAS_PROD_ESTAB" ADD PRIMARY KEY ("id_ventas_prod");

-- ----------------------------
-- Indexes structure for table DAT_VOLUMEN_PRODUCCION
-- ----------------------------

-- ----------------------------
-- Checks structure for table DAT_VOLUMEN_PRODUCCION
-- ----------------------------
ALTER TABLE "DAT_VOLUMEN_PRODUCCION" ADD CHECK ("id_volumen_prod" IS NOT NULL);
ALTER TABLE "DAT_VOLUMEN_PRODUCCION" ADD CHECK ("id_volumen_prod" IS NOT NULL);
ALTER TABLE "DAT_VOLUMEN_PRODUCCION" ADD CHECK ("id_volumen_prod" IS NOT NULL);
ALTER TABLE "DAT_VOLUMEN_PRODUCCION" ADD CHECK ("id_volumen_prod" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table DAT_VOLUMEN_PRODUCCION
-- ----------------------------
ALTER TABLE "DAT_VOLUMEN_PRODUCCION" ADD PRIMARY KEY ("id_volumen_prod");

-- ----------------------------
ALTER TABLE "SEG_ROL" ADD CHECK ("id_rol" IS NOT NULL);
ALTER TABLE "SEG_ROL" ADD CHECK ("nombre" IS NOT NULL);
ALTER TABLE "SEG_ROL" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "SEG_ROL" ADD CHECK ("id_rol" IS NOT NULL);
ALTER TABLE "SEG_ROL" ADD CHECK ("nombre" IS NOT NULL);
ALTER TABLE "SEG_ROL" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "SEG_ROL" ADD CHECK ("id_rol" IS NOT NULL);
ALTER TABLE "SEG_ROL" ADD CHECK ("nombre" IS NOT NULL);
ALTER TABLE "SEG_ROL" ADD CHECK ("estado" IS NOT NULL);
ALTER TABLE "SEG_ROL" ADD CHECK ("id_rol" IS NOT NULL);
ALTER TABLE "SEG_ROL" ADD CHECK ("nombre" IS NOT NULL);
ALTER TABLE "SEG_ROL" ADD CHECK ("estado" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table SEG_ROL
-- ----------------------------
ALTER TABLE "SEG_ROL" ADD PRIMARY KEY ("id_rol");

-- ----------------------------
-- Indexes structure for table SEG_USUARIO
-- ----------------------------

-- ----------------------------
-- Checks structure for table SEG_USUARIO
-- ----------------------------
ALTER TABLE "SEG_USUARIO" ADD CHECK ("id_usuario" IS NOT NULL);
ALTER TABLE "SEG_USUARIO" ADD CHECK ("login" IS NOT NULL);
ALTER TABLE "SEG_USUARIO" ADD CHECK ("id_usuario" IS NOT NULL);
ALTER TABLE "SEG_USUARIO" ADD CHECK ("login" IS NOT NULL);
ALTER TABLE "SEG_USUARIO" ADD CHECK ("id_usuario" IS NOT NULL);
ALTER TABLE "SEG_USUARIO" ADD CHECK ("login" IS NOT NULL);
ALTER TABLE "SEG_USUARIO" ADD CHECK ("id_usuario" IS NOT NULL);
ALTER TABLE "SEG_USUARIO" ADD CHECK ("login" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table SEG_USUARIO
-- ----------------------------
ALTER TABLE "SEG_USUARIO" ADD PRIMARY KEY ("id_usuario");

-- ----------------------------
-- Indexes structure for table SEG_USUARIO_ROL
-- ----------------------------

-- ----------------------------
-- Checks structure for table SEG_USUARIO_ROL
-- ----------------------------
ALTER TABLE "SEG_USUARIO_ROL" ADD CHECK ("id_usuario" IS NOT NULL);
ALTER TABLE "SEG_USUARIO_ROL" ADD CHECK ("id_rol" IS NOT NULL);
ALTER TABLE "SEG_USUARIO_ROL" ADD CHECK ("id_usuario" IS NOT NULL);
ALTER TABLE "SEG_USUARIO_ROL" ADD CHECK ("id_rol" IS NOT NULL);
ALTER TABLE "SEG_USUARIO_ROL" ADD CHECK ("id_usuario" IS NOT NULL);
ALTER TABLE "SEG_USUARIO_ROL" ADD CHECK ("id_rol" IS NOT NULL);
ALTER TABLE "SEG_USUARIO_ROL" ADD CHECK ("id_usuario" IS NOT NULL);
ALTER TABLE "SEG_USUARIO_ROL" ADD CHECK ("id_rol" IS NOT NULL);

-- ----------------------------
-- Primary Key structure for table SEG_USUARIO_ROL
-- ----------------------------
ALTER TABLE "SEG_USUARIO_ROL" ADD PRIMARY KEY ("id_usuario", "id_rol");

-- ----------------------------
-- Foreign Key structure for table "CAT_CONTACTO"
-- ----------------------------
ALTER TABLE "CAT_CONTACTO" ADD FOREIGN KEY ("id_cargo") REFERENCES "CAT_CARGO" ("id_cargo") ON DELETE SET NULL;
ALTER TABLE "CAT_CONTACTO" ADD FOREIGN KEY ("id_establecimiento") REFERENCES "CAT_ESTABLECIMIENTO" ("id_establecimiento") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Key structure for table "DAT_ENCUESTA"
-- ----------------------------
ALTER TABLE "DAT_ENCUESTA" ADD FOREIGN KEY ("id_establecimiento") REFERENCES "CAT_ESTABLECIMIENTO" ("id_establecimiento") ON DELETE CASCADE;
ALTER TABLE "DAT_ENCUESTA" ADD FOREIGN KEY ("id_informante") REFERENCES "SEG_USUARIO" ("id_usuario") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Key structure for table "DAT_ENCUESTA_EMPRESARIAL"
-- ----------------------------
ALTER TABLE "DAT_ENCUESTA_EMPRESARIAL" ADD FOREIGN KEY ("id_ENCUESTA") REFERENCES "DAT_ENCUESTA" ("id_ENCUESTA") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Key structure for table "DAT_ENCUESTA_ESTADISTICA"
-- ----------------------------
ALTER TABLE "DAT_ENCUESTA_ESTADISTICA" ADD FOREIGN KEY ("id_ENCUESTA") REFERENCES "DAT_ENCUESTA" ("id_ENCUESTA") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Key structure for table "CAT_ESTAB_CIIU"
-- ----------------------------
ALTER TABLE "CAT_ESTAB_CIIU" ADD FOREIGN KEY ("id_ciiu") REFERENCES "CAT_CIIU" ("id_ciiu") ON DELETE CASCADE;
ALTER TABLE "CAT_ESTAB_CIIU" ADD FOREIGN KEY ("id_establecimiento") REFERENCES "CAT_ESTABLECIMIENTO" ("id_establecimiento") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Key structure for table "CAT_ESTAB_LINEA_PROD"
-- ----------------------------
ALTER TABLE "CAT_ESTAB_LINEA_PROD" ADD FOREIGN KEY ("id_establecimiento") REFERENCES "CAT_ESTABLECIMIENTO" ("id_establecimiento") ON DELETE CASCADE;
ALTER TABLE "CAT_ESTAB_LINEA_PROD" ADD FOREIGN KEY ("id_linea_prod") REFERENCES "CAT_LINEA_PRODUCTO" ("id_linea_prod") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Key structure for table "CAT_ESTABLECIMIENTO"
-- ----------------------------
ALTER TABLE "CAT_ESTABLECIMIENTO" ADD FOREIGN KEY ("id_informante") REFERENCES "SEG_USUARIO" ("id_usuario") ON DELETE SET NULL;

-- ----------------------------
-- Foreign Key structure for table "DAT_FACTOR_FACTOR_PROD"
-- ----------------------------
ALTER TABLE "DAT_FACTOR_FACTOR_PROD" ADD FOREIGN KEY ("id_factor") REFERENCES "CAT_FACTOR" ("id_factor") ON DELETE CASCADE;
ALTER TABLE "DAT_FACTOR_FACTOR_PROD" ADD FOREIGN KEY ("id_factor_produccion") REFERENCES "DAT_FACTOR_PRODUCCION" ("id_factor_produccion") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Key structure for table "DAT_FACTOR_PRODUCCION"
-- ----------------------------
ALTER TABLE "DAT_FACTOR_PRODUCCION" ADD FOREIGN KEY ("id_factor_produccion") REFERENCES "DAT_ENCUESTA_ESTADISTICA" ("id_ENCUESTA") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Key structure for table "CAT_LINEA_PRODUCTO"
-- ----------------------------
ALTER TABLE "CAT_LINEA_PRODUCTO" ADD FOREIGN KEY ("id_ciiu") REFERENCES "CAT_CIIU" ("id_ciiu") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Key structure for table "DAT_MATERIA_PROPIA"
-- ----------------------------
ALTER TABLE "DAT_MATERIA_PROPIA" ADD FOREIGN KEY ("id_linea_producto") REFERENCES "CAT_LINEA_PRODUCTO" ("id_linea_prod") ON DELETE CASCADE;
ALTER TABLE "DAT_MATERIA_PROPIA" ADD FOREIGN KEY ("id_unidad_medida") REFERENCES "CAT_UNIDAD_MEDIDA" ("Id_unidad_medida") ON DELETE CASCADE;
ALTER TABLE "DAT_MATERIA_PROPIA" ADD FOREIGN KEY ("id_volumen_produccion") REFERENCES "DAT_VOLUMEN_PRODUCCION" ("id_volumen_prod") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Key structure for table "DAT_MATERIA_TERCEROS"
-- ----------------------------
ALTER TABLE "DAT_MATERIA_TERCEROS" ADD FOREIGN KEY ("id_linea_producto") REFERENCES "CAT_LINEA_PRODUCTO" ("id_linea_prod") ON DELETE CASCADE;
ALTER TABLE "DAT_MATERIA_TERCEROS" ADD FOREIGN KEY ("id_unidad_medida") REFERENCES "CAT_UNIDAD_MEDIDA" ("Id_unidad_medida") ON DELETE CASCADE;
ALTER TABLE "DAT_MATERIA_TERCEROS" ADD FOREIGN KEY ("id_volumen_prod") REFERENCES "DAT_VOLUMEN_PRODUCCION" ("id_volumen_prod") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Key structure for table "CAT_POSIBLE_RESPUESTA"
-- ----------------------------
ALTER TABLE "CAT_POSIBLE_RESPUESTA" ADD FOREIGN KEY ("id_pregunta") REFERENCES "DAT_PREGUNTA" ("id_pregunta") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Key structure for table "DAT_PREGUNTA"
-- ----------------------------
ALTER TABLE "DAT_PREGUNTA" ADD FOREIGN KEY ("id_encuesta_empresarial") REFERENCES "DAT_ENCUESTA_EMPRESARIAL" ("id_ENCUESTA") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Key structure for table "DAT_TRABAJADORES_DIAS_TRAB"
-- ----------------------------
ALTER TABLE "DAT_TRABAJADORES_DIAS_TRAB" ADD FOREIGN KEY ("id_trabajadores_dias_trabajado") REFERENCES "DAT_ENCUESTA_ESTADISTICA" ("id_ENCUESTA") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Key structure for table "CAT_VALOR"
-- ----------------------------
ALTER TABLE "CAT_VALOR" ADD FOREIGN KEY ("id_posible_respuesta") REFERENCES "CAT_POSIBLE_RESPUESTA" ("id_posible_respuesta") ON DELETE CASCADE;
ALTER TABLE "CAT_VALOR" ADD FOREIGN KEY ("id_pregunta") REFERENCES "DAT_PREGUNTA" ("id_pregunta") ON DELETE SET NULL;

-- ----------------------------
-- Foreign Key structure for table "DAT_VALOR_PROD_MENSUAL"
-- ----------------------------
ALTER TABLE "DAT_VALOR_PROD_MENSUAL" ADD FOREIGN KEY ("id_ciiu") REFERENCES "CAT_CIIU" ("id_ciiu") ON DELETE CASCADE;
ALTER TABLE "DAT_VALOR_PROD_MENSUAL" ADD FOREIGN KEY ("id_encuesta") REFERENCES "DAT_ENCUESTA_ESTADISTICA" ("id_ENCUESTA") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Key structure for table "DAT_VENTA_SERV_MANUF"
-- ----------------------------
ALTER TABLE "DAT_VENTA_SERV_MANUF" ADD FOREIGN KEY ("id_venta_prod_estab") REFERENCES "DAT_VENTAS_PROD_ESTAB" ("id_ventas_prod") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Key structure for table "DAT_VENTAS_PAIS_EXTRANJERO"
-- ----------------------------
ALTER TABLE "DAT_VENTAS_PAIS_EXTRANJERO" ADD FOREIGN KEY ("id_ciiu") REFERENCES "CAT_CIIU" ("id_ciiu") ON DELETE CASCADE;
ALTER TABLE "DAT_VENTAS_PAIS_EXTRANJERO" ADD FOREIGN KEY ("id_ventas_producto") REFERENCES "DAT_VENTAS_PROD_ESTAB" ("id_ventas_prod") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Key structure for table "DAT_VENTAS_PROD_ESTAB"
-- ----------------------------
ALTER TABLE "DAT_VENTAS_PROD_ESTAB" ADD FOREIGN KEY ("id_ventas_prod") REFERENCES "DAT_ENCUESTA_ESTADISTICA" ("id_ENCUESTA") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Key structure for table "DAT_VOLUMEN_PRODUCCION"
-- ----------------------------
ALTER TABLE "DAT_VOLUMEN_PRODUCCION" ADD FOREIGN KEY ("id_volumen_prod") REFERENCES "DAT_ENCUESTA_ESTADISTICA" ("id_ENCUESTA") ON DELETE CASCADE;

-- ----------------------------
-- Foreign Key structure for table "SEG_USUARIO_ROL"
-- ----------------------------
ALTER TABLE "SEG_USUARIO_ROL" ADD FOREIGN KEY ("id_rol") REFERENCES "SEG_ROL" ("id_rol") ON DELETE CASCADE;
ALTER TABLE "SEG_USUARIO_ROL" ADD FOREIGN KEY ("id_usuario") REFERENCES "SEG_USUARIO" ("id_usuario") ON DELETE CASCADE;

--TIPO CAMBIO
CREATE TABLE "CAT_TIPO_CAMBIO" (
"id_cambio" NUMBER(11) NOT NULL ,
"fecha" DATE NOT NULL,
"tipo_cambio_venta" NUMBER NOT NULL,
"tipo_cambio_compra" NUMBER NOT NULL,
"estado" NUMBER DEFAULT 0  NOT NULL,
"creado" DATE NULL ,
"modificado" DATE NULL ,
"usuario_creacion" VARCHAR2(50 BYTE) NULL ,
"usuario_modificacion" VARCHAR2(50 BYTE) NULL 
)
LOGGING
NOCOMPRESS
NOCACHE
;
ALTER TABLE "CAT_TIPO_CAMBIO" ADD PRIMARY KEY ("id_cambio");

--IPM_IPP
CREATE TABLE "DAT_IPM_IPP" (
"id_ipm_ipp" NUMBER(11) NOT NULL ,
"fecha" DATE NOT NULL,
"ipm" NUMBER NOT NULL,
"ipp" NUMBER NOT NULL,
"estado" NUMBER DEFAULT 0  NOT NULL,
"id_ciiu" NUMBER(11) NOT NULL
)
LOGGING
NOCOMPRESS
NOCACHE
;
ALTER TABLE "DAT_IPM_IPP" ADD PRIMARY KEY ("id_ipm_ipp");

ALTER TABLE "DAT_IPM_IPP" ADD FOREIGN KEY ("id_ciiu") REFERENCES "CAT_CIIU" ("id_ciiu") ON DELETE CASCADE;

--CONSUMO HARINA PARA FIDEO
CREATE TABLE "CAT_CONSUMO_HARINA_FIDEO" (
"id_consumo" NUMBER(11) NOT NULL ,
"fecha" DATE NOT NULL,
"tonelada_tmb" NUMBER NOT NULL,
"estado" NUMBER DEFAULT 0  NOT NULL,
"creado" DATE NULL ,
"modificado" DATE NULL ,
"usuario_creacion" VARCHAR2(50 BYTE) NULL ,
"usuario_modificacion" VARCHAR2(50 BYTE) NULL 
)
LOGGING
NOCOMPRESS
NOCACHE
;
ALTER TABLE "CAT_CONSUMO_HARINA_FIDEO" ADD PRIMARY KEY ("id_consumo");

--EXPORTACION HARINA DE TRIGO
CREATE TABLE "CAT_EXPORTACION_HARINA_TRIGO" (
"id_exportacion" NUMBER(11) NOT NULL ,
"fecha" DATE NOT NULL,
"fob_usd" NUMBER NOT NULL,
"fob_s" NUMBER NOT NULL,
"estado" NUMBER DEFAULT 0  NOT NULL
)
LOGGING
NOCOMPRESS
NOCACHE
;
ALTER TABLE "CAT_EXPORTACION_HARINA_TRIGO" ADD PRIMARY KEY ("id_exportacion");

--IMPORTACION HARINA DE TRIGO
CREATE TABLE "CAT_IMPORTACION_HARINA_TRIGO" (
"id_importacion" NUMBER(11) NOT NULL ,
"fecha" DATE NOT NULL,
"cif_usd" NUMBER NOT NULL,
"cif_s" NUMBER NOT NULL,
"estado" NUMBER DEFAULT 0  NOT NULL
)
LOGGING
NOCOMPRESS
NOCACHE
;
ALTER TABLE "CAT_IMPORTACION_HARINA_TRIGO" ADD PRIMARY KEY ("id_importacion");

--METODO CALCULO
CREATE TABLE "CAT_METODO_CALCULO" (
"id_metodo" NUMBER(11) NOT NULL ,
"nombre" VARCHAR2(20 BYTE),
"estado" NUMBER DEFAULT 0  NOT NULL,
"creado" DATE NULL ,
"modificado" DATE NULL ,
"usuario_creacion" VARCHAR2(50 BYTE) NULL ,
"usuario_modificacion" VARCHAR2(50 BYTE) NULL 
)
LOGGING
NOCOMPRESS
NOCACHE
;
ALTER TABLE "CAT_METODO_CALCULO" ADD PRIMARY KEY ("id_metodo");
ALTER TABLE "CAT_METODO_CALCULO" ADD("registro_obligatorio" NUMBER(1) NULL);

ALTER TABLE "CAT_CIIU" ADD FOREIGN KEY ("id_metodo_calculo") REFERENCES "CAT_METODO_CALCULO" ("id_metodo") ON DELETE CASCADE;

CREATE TABLE "CAT_LINEA_PRODTO_UNIDAD_MEDIDA" (
"id_linea_producto_um" NUMBER(11) NOT NULL ,
"id_unidad_conversion" NUMBER(11) NULL,
"factor_conversion" NUMBER DEFAULT 1 NOT NULL,
"id_unidad_medida" NUMBER(11) NULL,
"id_linea_producto" NUMBER(11) NULL,
"creado" DATE NULL ,
"modificado" DATE NULL ,
"usuario_creacion" VARCHAR2(50 BYTE) NULL ,
"usuario_modificacion" VARCHAR2(50 BYTE) NULL 
);

ALTER TABLE "CAT_LINEA_PRODTO_UNIDAD_MEDIDA" ADD PRIMARY KEY ("id_linea_producto_um");

ALTER TABLE "CAT_LINEA_PRODTO_UNIDAD_MEDIDA" ADD FOREIGN KEY ("id_unidad_conversion") REFERENCES "CAT_UNIDAD_MEDIDA" ("Id_unidad_medida") ON DELETE SET NULL;
ALTER TABLE "CAT_LINEA_PRODTO_UNIDAD_MEDIDA" ADD FOREIGN KEY ("id_unidad_medida") REFERENCES "CAT_UNIDAD_MEDIDA" ("Id_unidad_medida") ON DELETE CASCADE;
ALTER TABLE "CAT_LINEA_PRODTO_UNIDAD_MEDIDA" ADD FOREIGN KEY ("id_linea_producto") REFERENCES "CAT_LINEA_PRODUCTO" ("id_linea_prod") ON DELETE CASCADE;

--AÑO BASE
CREATE TABLE "CAT_ANIO_BASE" (
"id_año" NUMBER(11) NOT NULL ,
"id_establecimiento" NUMBER(11) NOT NULL ,
"id_ciiu" NUMBER(11) NOT NULL ,
"id_linea_producto" NUMBER(11)  NULL ,
"id_unidad_medida" NUMBER(11)  NULL ,
"produccion_anual" NUMBER NOT NULL,
"valor_produccion" NUMBER NOT NULL,
"precio" NUMBER NOT NULL,
"estado" NUMBER DEFAULT 0  NOT NULL,
"creado" DATE NULL ,
"modificado" DATE NULL ,
"usuario_creacion" VARCHAR2(50 BYTE) NULL ,
"usuario_modificacion" VARCHAR2(50 BYTE) NULL  
)
LOGGING
NOCOMPRESS
NOCACHE
;
ALTER TABLE "CAT_ANIO_BASE" ADD PRIMARY KEY ("id_año");
ALTER TABLE "CAT_ANIO_BASE" ADD FOREIGN KEY ("id_establecimiento") REFERENCES "CAT_ESTABLECIMIENTO" ("id_establecimiento") ON DELETE CASCADE;
ALTER TABLE "CAT_ANIO_BASE" ADD FOREIGN KEY ("id_ciiu") REFERENCES "CAT_CIIU" ("id_ciiu") ON DELETE CASCADE;
ALTER TABLE "CAT_ANIO_BASE" ADD FOREIGN KEY ("id_linea_producto") REFERENCES "CAT_LINEA_PRODUCTO" ("id_linea_prod") ON DELETE CASCADE;
ALTER TABLE "CAT_ANIO_BASE" ADD FOREIGN KEY ("id_unidad_medida") REFERENCES "CAT_UNIDAD_MEDIDA" ("Id_unidad_medida") ON DELETE CASCADE;

--PARAMETRIZACION DE ENVIO
CREATE TABLE "CAT_PARAMETRIZACION_ENVIO" (
"id_parametrizacion" NUMBER(11) NOT NULL ,
"tipo_encuesta" VARCHAR2(50 BYTE) NOT NULL,
"mensaje" VARCHAR2(1000 BYTE) NOT NULL,
"comienzo" DATE NOT NULL,
"envio_1" NUMBER(11) NOT NULL ,
"envio_2" NUMBER(11) NOT NULL ,
"estado" NUMBER DEFAULT 0  NOT NULL,
"frecuencia" VARCHAR2(50 BYTE)  NULL,
"creado" DATE NULL ,
"modificado" DATE NULL ,
"usuario_creacion" VARCHAR2(50 BYTE) NULL ,
"usuario_modificacion" VARCHAR2(50 BYTE) NULL 
)
LOGGING
NOCOMPRESS
NOCACHE
;
ALTER TABLE "CAT_PARAMETRIZACION_ENVIO" ADD PRIMARY KEY ("id_parametrizacion");

--AUDITORIA
CREATE TABLE "DAT_ENCUESTA_AUDITORIA" (
"id_auditoria" NUMBER(11) NOT NULL ,
"id_encuesta" NUMBER(11) NOT NULL,
"accion" VARCHAR2(50 BYTE)  NULL,
"usuario" VARCHAR2(50 BYTE) NOT NULL,
"fecha" DATE NOT NULL
)
LOGGING
NOCOMPRESS
NOCACHE
;
ALTER TABLE "DAT_ENCUESTA_AUDITORIA" ADD PRIMARY KEY ("id_auditoria");
ALTER TABLE "DAT_ENCUESTA_AUDITORIA" ADD FOREIGN KEY ("id_encuesta") REFERENCES "DAT_ENCUESTA" ("id_ENCUESTA") ON DELETE SET NULL;

CREATE TABLE "CAT_ESTAB_ANALISTA" (
"id_establecimiento_analista" NUMBER(11) NOT NULL ,
"orden" NUMBER NOT NULL,
"id_establecimiento" NUMBER(11) NOT NULL ,
"id_analista" NUMBER NOT NULL ,
"id_ciiu" NUMBER(11) NOT NULL,
"creado" DATE NULL ,
"modificado" DATE NULL ,
"usuario_creacion" VARCHAR2(50 BYTE) NULL ,
"usuario_modificacion" VARCHAR2(50 BYTE) NULL 
)
LOGGING
NOCOMPRESS
NOCACHE
;
ALTER TABLE "CAT_ESTAB_ANALISTA" ADD PRIMARY KEY ("id_establecimiento_analista");
ALTER TABLE "CAT_ESTAB_ANALISTA" ADD FOREIGN KEY ("id_establecimiento") REFERENCES "CAT_ESTABLECIMIENTO" ("id_establecimiento") ON DELETE CASCADE;
ALTER TABLE "CAT_ESTAB_ANALISTA" ADD FOREIGN KEY ("id_ciiu") REFERENCES "CAT_CIIU" ("id_ciiu") ON DELETE CASCADE;
ALTER TABLE "CAT_ESTAB_ANALISTA" ADD FOREIGN KEY ("id_analista") REFERENCES "SEG_USUARIO" ("id_usuario") ON DELETE CASCADE;

CREATE TABLE "DAT_ENCUESTA_ANALISTA" (
"id_encuesta_analista" NUMBER(11) NOT NULL ,
"orden" NUMBER NOT NULL,
"id_encuesta" NUMBER(11) NOT NULL ,
"id_analista" NUMBER NOT NULL ,
"id_ciiu" NUMBER(11) NOT NULL,
"estado" NUMBER DEFAULT 0 NOT NULL,
"current" NUMBER DEFAULT 0 NOT NULL

)
LOGGING
NOCOMPRESS
NOCACHE
;
ALTER TABLE "DAT_ENCUESTA_ANALISTA" ADD PRIMARY KEY ("id_encuesta_analista");
ALTER TABLE "DAT_ENCUESTA_ANALISTA" ADD FOREIGN KEY ("id_encuesta") REFERENCES "DAT_ENCUESTA" ("id_ENCUESTA") ON DELETE CASCADE;
ALTER TABLE "DAT_ENCUESTA_ANALISTA" ADD FOREIGN KEY ("id_ciiu") REFERENCES "CAT_CIIU" ("id_ciiu") ON DELETE CASCADE;
ALTER TABLE "DAT_ENCUESTA_ANALISTA" ADD FOREIGN KEY ("id_analista") REFERENCES "SEG_USUARIO" ("id_usuario") ON DELETE CASCADE;

CREATE VIEW CAT_PORCENTAJE_ENC_EST
AS
select ENC."id_ENCUESTA" as "id_encuesta",
ENC."estado_ENCUESTA" as "estado_encuesta",
ENC."id_establecimiento",
ENC."fecha",
ENCA."id_ciiu",
ENCA."id_analista",
ENCA."estado" as "estado_validacion",
CIIU."codigo" as "codigo_ciiu",
CIIU."nombre" as "nombre_ciiu",
USUARIO."login" as "login_analista"
from (((DAT_ENCUESTA ENC JOIN DAT_ENCUESTA_ESTADISTICA ENCES ON ENC."id_ENCUESTA"=ENCES."id_ENCUESTA")
JOIN DAT_ENCUESTA_ANALISTA ENCA ON ENCA."id_encuesta"=ENC."id_ENCUESTA")
JOIN CAT_CIIU CIIU ON CIIU."id_ciiu"=ENCA."id_ciiu")
JOIN SEG_USUARIO USUARIO ON USUARIO."id_usuario"=ENCA."id_analista";