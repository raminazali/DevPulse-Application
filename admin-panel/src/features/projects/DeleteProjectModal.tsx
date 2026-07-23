import { useMutation, useQueryClient } from "@tanstack/react-query";
import { projectService } from "../../shared/services/project.service";
import { Modal } from "../../shared/components/Modal";
import type { ProjectListItemDto } from "../../shared/types/ProjectListItemDto";

interface DeleteProjectModalProps {
  open: boolean;
  project: ProjectListItemDto | null;
  onClose: () => void;
  onSuccess?: () => void;
}

export function DeleteProjectModal({
  open,
  project,
  onClose,
  onSuccess,
}: DeleteProjectModalProps) {
  const queryClient = useQueryClient();

  const deleteMutation = useMutation({
    mutationFn: (id: string) => projectService.deleteProject(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["projects"] });
      onClose();
      onSuccess?.();
    },
  });

  const handleDelete = () => {
    if (project) {
      deleteMutation.mutate(project.id);
    }
  };

  return (
    <Modal open={open} title="حذف پروژه" onClose={onClose} size="sm">
      <div className="space-y-4">
        <p>
          آیا از حذف پروژه <strong>{project?.name}</strong> اطمینان دارید؟
        </p>
        <p className="text-sm opacity-70">این عمل غیرقابل بازگشت است.</p>

        <div className="flex justify-end gap-2 pt-4">
          <button
            type="button"
            className="btn btn-ghost rounded-xl"
            onClick={onClose}
            disabled={deleteMutation.isPending}
          >
            انصراف
          </button>
          <button
            type="button"
            className="btn rounded-xl border-0 bg-error text-error-content shadow-md transition-all duration-200 hover:scale-[1.02] hover:brightness-95 active:scale-[0.98]"
            onClick={handleDelete}
            disabled={deleteMutation.isPending}
          >
            {deleteMutation.isPending ? "در حال حذف..." : "حذف"}
          </button>
        </div>
      </div>
    </Modal>
  );
}
