BEGIN;

ALTER TABLE public.usuarios
    ADD COLUMN IF NOT EXISTS auth_user_id uuid;

DO $$
BEGIN
    IF EXISTS (
        SELECT 1
        FROM public.usuarios u
        JOIN auth.users a ON lower(trim(u.email)) = lower(trim(a.email))
        GROUP BY lower(trim(u.email))
        HAVING COUNT(DISTINCT u.id_usuario) > 1
    ) THEN
        RAISE EXCEPTION 'Multiple public.usuarios records match the same Supabase Auth email; resolve duplicates before applying this migration.';
    END IF;

    IF EXISTS (
        SELECT 1
        FROM public.usuarios u
        JOIN auth.users a ON lower(trim(u.email)) = lower(trim(a.email))
        WHERE u.auth_user_id IS NOT NULL
          AND u.auth_user_id <> a.id
    ) THEN
        RAISE EXCEPTION 'A public.usuarios record is already linked to a different Supabase Auth user.';
    END IF;
END $$;

UPDATE public.usuarios u
SET auth_user_id = a.id
FROM auth.users a
WHERE lower(trim(u.email)) = lower(trim(a.email))
  AND u.auth_user_id IS NULL;

CREATE UNIQUE INDEX IF NOT EXISTS ix_usuarios_auth_user_id
    ON public.usuarios (auth_user_id)
    WHERE auth_user_id IS NOT NULL;

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM pg_constraint
        WHERE conname = 'fk_usuarios_auth_user_id'
          AND conrelid = 'public.usuarios'::regclass
    ) THEN
        ALTER TABLE public.usuarios
            ADD CONSTRAINT fk_usuarios_auth_user_id
            FOREIGN KEY (auth_user_id)
            REFERENCES auth.users (id)
            ON DELETE SET NULL;
    END IF;
END $$;

COMMIT;
