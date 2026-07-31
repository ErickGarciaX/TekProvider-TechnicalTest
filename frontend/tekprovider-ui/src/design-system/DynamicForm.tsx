import { zodResolver } from '@hookform/resolvers/zod';
import { Button, Stack, TextInput } from '@mantine/core';
import { useForm, type FieldValues, type Path, type Resolver } from 'react-hook-form';
import type { ZodType } from 'zod';

export interface DynamicFieldSchema<TValues> {
  name: Path<TValues>;
  label: string;
  type: 'text' | 'email' | 'tel' | 'password';
  required?: boolean;
  placeholder?: string;
}

interface DynamicFormProps<TValues extends FieldValues> {
  fields: DynamicFieldSchema<TValues>[];
  schema: ZodType<TValues>;
  defaultValues: TValues;
  submitLabel: string;
  onSubmit: (values: TValues) => void;
  isSubmitting?: boolean;
}

export function DynamicForm<TValues extends FieldValues>({
  fields,
  schema,
  defaultValues,
  submitLabel,
  onSubmit,
  isSubmitting,
}: DynamicFormProps<TValues>) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<TValues>({
    // zod v4's generic ZodType internals don't line up with @hookform/resolvers' overloads
    // for a generic TValues — this cast is purely a type-level interop shim, runtime is unaffected.
    resolver: zodResolver(schema as never) as Resolver<TValues>,
    defaultValues: defaultValues as never,
  });

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <Stack>
        {fields.map((field) => (
          <TextInput
            key={String(field.name)}
            label={field.label}
            type={field.type}
            required={field.required}
            placeholder={field.placeholder}
            {...register(field.name)}
            error={errors[field.name]?.message as string | undefined}
          />
        ))}
        <Button type="submit" loading={isSubmitting} fullWidth>
          {submitLabel}
        </Button>
      </Stack>
    </form>
  );
}
