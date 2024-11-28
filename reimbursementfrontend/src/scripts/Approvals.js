import axiosIntance from "./Interceptor";

export function getAllApprovals(pageNumber, pageSize) {
  return axiosIntance.get("/api/Approval/approvals", {
    params: {
      pageNumber,
      pageSize,
    },
  });
}

export function approveRequest(requestId, reviewId, comments) {
  return axiosIntance.post("/api/Approval/approve", {
    requestId,
    reviewId,
    comments,
  });
}

export function rejectRequest(requestId, reviewId, comments) {
  return axiosIntance.post("/api/Approval/reject", {
    requestId,
    reviewId,
    comments,
  });
}

export function getRequestForHR(pageNumber, pageSize) {
  return axiosIntance.get("/api/Approval/hr", {
    params: {
      pageNumber,
      pageSize,
    },
  });
}

export function getRequestForFinance(pageNumber, pageSize) {
  return axiosIntance.get("/api/Approval/finance", {
    params: {
      pageNumber,
      pageSize,
    },
  });
}
