-- Table: public."__EFMigrationsHistory"

-- DROP TABLE public."__EFMigrationsHistory";

CREATE TABLE public."__EFMigrationsHistory"
(
    "MigrationId" character varying(150) COLLATE pg_catalog."default" NOT NULL,
    "ProductVersion" character varying(32) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
)

TABLESPACE pg_default;

ALTER TABLE public."__EFMigrationsHistory"
    OWNER to postgres;
	
INSERT INTO public."__EFMigrationsHistory"(
	"MigrationId", "ProductVersion")
	VALUES ('20200217194901_Migration_0', '3.0.1');
