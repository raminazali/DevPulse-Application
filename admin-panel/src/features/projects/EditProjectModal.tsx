import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { projectService } from "../../shared/services/project.service";
import { Modal } from "../../shared/components/Modal";
import { FormInput } from "../../shared/components/FormInput";
import { FormSwitch } from "../../shared/components/FormSwitch";
import type { ProjectListItemDto } from "../../shared/types/ProjectListItemDto";

const editProjectSchema = z.object({
  name: z.string().min(3, "نام پروژه باید حداقل ۳ حرف باشد"),
  isActive: z.boolean(),
});

type EditProjectForm = z.infer<typeof editProjectSchema>;

interface EditProjectModalProps {
  open: boolean;
  project: ProjectListItemDto;
  onClose: () => void;
  onSuccess?: () => void;
}

export function EditProjectModal({
  open,
  project,
  onClose,
  onSuccess,
}: EditProjectModalProps) {
  const queryClient = useQueryClient();

  const {
    control,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<EditProjectForm>({
    resolver: zodResolver(editProjectSchema),
    defaultValues: {
      name: project.name,
      isActive: project.isActive,
    },
  });

  useEffect(() => {
    if (open) {
      reset({
        name: project.name,
        isActive: project.isActive,
      });
    }
  }, [open, project, reset]);

  const updateMutation = useMutation({
    mutationFn: (data: EditProjectForm & { id: string }) =>
      projectService.updateProject(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["projects"] });
      onClose();
      onSuccess?.();
    },
  });

  const onSubmit = (data: EditProjectForm) => {
    updateMutation.mutate({ id: project.id, ...data });
  };

  const isPending = updateMutation.isPending;

  return (
    <Modal open={open} title="ویرایش پروژه" onClose={onClose} size="md">
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
        <FormInput<EditProjectForm>
          label="نام پروژه"
          name="name"
          control={control}
          error={errors.name}
        />

        <FormSwitch<EditProjectForm>
          label="وضعیت فعال"
          name="isActive"
          control={control}
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
            {isPending ? "در حال ذخیره..." : "ذخیره تغییرات"}
          </button>
        </div>
      </form>
    </Modal>
  );
}
