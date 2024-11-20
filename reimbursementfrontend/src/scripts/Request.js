import axiosIntance from "./Interceptor";

export function getRequests(pageNumber = 1, pageSize = 10) {
  return axiosIntance.get("api/ReimbursementRequest/all", {
    params: {
      pageNumber,
      pageSize,
    },
  });
}

export function getDashboard() {
  return axiosIntance.get("api/ReimbursementRequest/dashboard");
}
