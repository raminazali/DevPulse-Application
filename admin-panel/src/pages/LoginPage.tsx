import { Mail, Lock, Eye, EyeOff, Zap } from "lucide-react";
import { Navigate, useNavigate } from "react-router-dom";
import { useAuth } from "../shared/hooks/useAuth";
import z from "zod";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation } from "@tanstack/react-query";
import { FormInput } from "../shared/components/FormInput";
import ThemeToggle from "../shared/utils/ThemeToggle";
import { useState } from "react";

const loginSchema = z.object({
  email: z.string().email("ایمیل معتبر نیست"),
  password: z.string().min(4, "رمز عبور باید حداقل ۴ کاراکتر باشد"),
});

type LoginType = z.infer<typeof loginSchema>;

export default function LoginPageFa() {
  const { login, user, loading } = useAuth();
  const navigate = useNavigate();
  const [showPassword, setShowPassword] = useState(false);

  const {
    control,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginType>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      email: "",
      password: "",
    },
  });

  const loginMutation = useMutation({
    mutationFn: (data: LoginType) => login(data.email, data.password),
    onSuccess: () => {
      navigate("/", { replace: true });
    },
  });

  const onSubmit = (data: LoginType) => {
    loginMutation.mutate(data);
  };

  if (!loading && user && localStorage.getItem("artemis-token")) {
    return <Navigate to="/" replace />;
  }

  return (
    <div
      dir="rtl"
      className="relative flex min-h-screen items-center justify-center overflow-hidden bg-base-200 px-4"
    >
      <div className="pointer-events-none absolute -top-32 -right-32 h-[28rem] w-[28rem] rounded-full bg-primary/20 blur-3xl" />
      <div className="pointer-events-none absolute -bottom-32 -left-32 h-[28rem] w-[28rem] rounded-full bg-primary/10 blur-3xl" />

      <div className="absolute top-4 left-4 z-20">
        <ThemeToggle />
      </div>

      <div className="relative z-10 w-full max-w-md">
        <div className="mb-8 text-center">
          <div className="mx-auto mb-4 flex h-14 w-14 items-center justify-center rounded-2xl bg-primary text-primary-content shadow-lg shadow-primary/25">
            <Zap size={28} />
          </div>
          <h1 className="text-2xl font-semibold tracking-tight">دوپالس</h1>
          <p className="saas-card-subtitle mt-1">ورود به پنل مدیریت</p>
        </div>

        <form
          onSubmit={handleSubmit(onSubmit)}
          className="saas-card space-y-5 shadow-xl"
        >
          <FormInput<LoginType>
            label="ایمیل"
            name="email"
            type="email"
            placeholder="you@company.com"
            control={control}
            error={errors.email}
            startIcon={<Mail className="h-5 w-5" />}
          />

          <FormInput<LoginType>
            label="رمز عبور"
            name="password"
            type={showPassword ? "text" : "password"}
            placeholder="••••••••"
            control={control}
            error={errors.password}
            startIcon={<Lock className="h-5 w-5" />}
            endAdornment={
              <button
                type="button"
                onClick={() => setShowPassword(!showPassword)}
                className="text-base-content/40 transition-colors duration-200 hover:text-base-content"
              >
                {showPassword ? (
                  <EyeOff className="h-5 w-5" />
                ) : (
                  <Eye className="h-5 w-5" />
                )}
              </button>
            }
          />

          <button
            type="submit"
            className="btn-saas btn w-full"
            disabled={loginMutation.isPending}
          >
            {loginMutation.isPending ? "در حال ورود..." : "ورود"}
          </button>
        </form>
      </div>
    </div>
  );
}
