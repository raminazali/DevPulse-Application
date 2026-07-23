import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Modal } from "../../shared/components/Modal";
import { FormInput } from "../../shared/components/FormInput";
import { userService } from "../../shared/services/user.service";

const createUserSchema = z.object({
  fullName: z.string().min(3, "نام کامل باید حداقل ۳ حرف باشد"),
  email: z.string().email("ایمیل معتبر وارد کنید"),
  password: z.string().min(6, "رمز عبور باید حداقل ۶ حرف باشد"),
});

type CreateUserForm = z.infer<typeof createUserSchema>;

interface CreateUserModalProps {
  open: boolean;
  onClose: () => void;
  onSuccess?: () => void;
}

export function CreateUserModal({
  open,
  onClose,
  onSuccess,
}: CreateUserModalProps) {
  const queryClient = useQueryClient();

  const {
    control,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<CreateUserForm>({
    resolver: zodResolver(createUserSchema),
    defaultValues: {
      fullName: "",
      email: "",
      password: "",
    },
  });

  const createMutation = useMutation({
    mutationFn: (data: CreateUserForm) => userService.createUser(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["users"] });
      reset();
      onClose();
      onSuccess?.();
    },
  });

  const onSubmit = (data: CreateUserForm) => {
    createMutation.mutate(data);
  };

  const isPending = createMutation.isPending;

  return (
    <Modal open={open} title="ایجاد کاربر جدید" onClose={onClose} size="md">
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
        <FormInput<CreateUserForm>
          label="نام کامل"
          name="fullName"
          control={control}
          error={errors.fullName}
        />

        <FormInput<CreateUserForm>
          label="ایمیل"
          name="email"
          type="email"
          control={control}
          error={errors.email}
        />

        <FormInput<CreateUserForm>
          label="رمز عبور"
          name="password"
          type="password"
          control={control}
          error={errors.password}
        />

        <div className="flex justify-end gap-2 pt-4">
          <button
            type="button"
            className="btn btn-ghost rounded-xl"
            onClick={onClose}
            disabled={isPending}
          >
            انصراف
          </button>
          <button type="submit" className="btn-saas btn" disabled={isPending}>
            {isPending ? "در حال ایجاد..." : "ایجاد کاربر"}
          </button>
        </div>
      </form>
    </Modal>
  );
}
