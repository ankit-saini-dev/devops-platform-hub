BEGIN;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM flyway_schema_history
        WHERE script = 'V20260911115526493500__create_platform_schema.sql'
          AND success
    ) THEN
        RAISE EXCEPTION 'Flyway migration V20260911115526493500 was not applied successfully.';
    END IF;

    IF NOT EXISTS (
        SELECT 1
        FROM information_schema.schemata
        WHERE schema_name = 'platform'
    ) THEN
        RAISE EXCEPTION 'The platform schema does not exist.';
    END IF;
END;
$$;

CREATE TEMPORARY TABLE relational_smoke (
    id integer PRIMARY KEY,
    name text NOT NULL UNIQUE
);

INSERT INTO relational_smoke (id, name)
VALUES (1, 'first');

DO $$
BEGIN
    INSERT INTO relational_smoke (id, name)
    VALUES (2, 'first');

    RAISE EXCEPTION 'Expected unique constraint violation was not raised.';
EXCEPTION
    WHEN unique_violation THEN NULL;
END;
$$;

ROLLBACK;
