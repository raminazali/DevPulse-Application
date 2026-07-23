import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { projectService } from "../../shared/services/project.service";
import { Modal } from "../../shared/components/Modal";
import { FormInput } from "../../shared/components/FormInput";
import { FormAsyncSelect } from "../../shared/components/FormAsyncSelect";

const createProjectSchema = z.object({
  userId: z.string().uuid("شناسه کاربر معتبر نیست"),
  name: z.string().min(3, "نام پروژه باید حداقل ۳ حرف باشد"),
});

type CreateProjectForm = z.infer<typeof createProjectSchema>;

interface CreateProjectModalProps {
  open: boolean;
  onClose: () => void;
  onSuccess?: () => void;
}

export function CreateProjectModal({
  open,
  onClose,
  onSuccess,
}: CreateProjectModalProps) {
  const queryClient = useQueryClient();

  const {
    control,
    register,
    handleSubmit,
    reset,
    setValue,
    watch,
    formState: { errors },
  } = useForm<CreateProjectForm>({
    resolver: zodResolver(createProjectSchema),
    defaultValues: {
      userId: "",
      name: "",
    },
  });

  const createMutation = useMutation({
    mutationFn: (data: CreateProjectForm) => projectService.createProject(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["projects"] });
      reset();
      onClose();
      onSuccess?.();
    },
  });

  const onSubmit = (data: CreateProjectForm) => {
    createMutation.mutate(data);
  };

  const isPending = createMutation.isPending;

  return (
    <Modal open={open} title="ایجاد پروژه جدید" onClose={onClose} size="md">
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
        <FormAsyncSelect<CreateProjectForm>
          label="شناسه کاربر"
          name="userId"
          register={register}
          watch={watch}
          setValue={setValue}
          error={errors.userId}
        />

        <FormInput<CreateProjectForm>
          label="نام پروژه"
          name="name"
          control={control}
          error={errors.name}
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
            {isPending ? "در حال ایجاد..." : "ایجاد پروژه"}
          </button>
        </div>
      </form>
    </Modal>
  );
}
