export function formatToIranDateOnly(date: string | Date) {
  return new Intl.DateTimeFormat("fa-IR", {
    timeZone: "Asia/Tehran",
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
  }).format(new Date(date));
}

export function formatToIranDateTime(date: string | Date) {
  return new Intl.DateTimeFormat("fa-IR", {
    timeZone: "Asia/Tehran",
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
  }).format(new Date(date));
}
export function formatToRelativeTime(date: string | Date): string {
  const target = new Date(date);
  const now = new Date();

  const diff = now.getTime() - target.getTime();

  const seconds = Math.floor(diff / 1000);

  if (seconds < 60) {
    return "همین الان";
  }
  
  const minutes = Math.floor(seconds / 60);

  if (minutes < 60) {
    return `${minutes} دقیقه پیش`;
  }

  const hours = Math.floor(minutes / 60);

  if (hours < 24) {
    return `${hours} ساعت پیش`;
  }

  const days = Math.floor(hours / 24);

  if (days < 30) {
    return `${days} روز پیش`;
  }

  const months = Math.floor(days / 30);

  if (months < 12) {
    return `${months} ماه پیش`;
  }

  const years = Math.floor(months / 12);

  return `${years} سال پیش`;
}
export function formatRelativeWithDate(date: string | Date): string {
  return `${formatToRelativeTime(date)} · ${formatToIranDateTime(date)}`;
}
