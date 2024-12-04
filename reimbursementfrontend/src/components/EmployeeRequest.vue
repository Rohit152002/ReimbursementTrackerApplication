<template>
    <div class="p-8">
        <h1 class="text-2xl font-bold mb-6">Employee Requests</h1>
        <div class="flex justify-between items-center bg-white p-4 shadow rounded-lg">
            <h2 class="text-xl font-semibold text-gray-800">Welcome, {{ username }}</h2>

        </div>

        <div v-if="department == 'HR' || department == 'Finance'" class="overflow-x-auto">
            <div class="mt-4 flex justify-between items-center">
                <input v-model="searchQuery" @input="filterRequests" placeholder="Search requests..."
                    class="border border-gray-300 rounded-lg p-2 w-1/2 focus:outline-none focus:ring-2 focus:ring-blue-500" />
                <div class="flex gap-4">

                    <button @click="toggleStatusFilter('Pending')" :class="getStatusButtonClass('Pending')">
                        Pending
                    </button>
                    <button @click="toggleStatusFilter('Passed')" :class="getStatusButtonClass('Passed')">
                        Passed
                    </button>
                    <button @click="toggleStatusFilter('Rejected')" :class="getStatusButtonClass('Rejected')">
                        Rejected
                    </button>

                </div>

            </div>
            <table class="min-w-full border-collapse bg-white shadow-md rounded-lg">
                <thead>
                    <tr class="bg-blue-100 text-gray-700">
                        <th class="py-3 px-4 border-b text-sm font-semibold">#</th>
                        <th class="py-3 px-4 border-b text-sm font-semibold">Employee Name</th>
                        <th class="py-3 px-4 border-b text-sm font-semibold">Department</th>
                        <th class="py-3 px-4 border-b text-sm font-semibold">Policy</th>
                        <th class="py-3 px-4 border-b text-sm font-semibold">Total Amount</th>
                        <th class="py-3 px-4 border-b text-sm font-semibold">Comments</th>
                        <th class="py-3 px-4 border-b text-sm font-semibold">Reviewer</th>
                        <th class="py-3 px-4 border-b text-sm font-semibold">Status</th>
                        <th class="py-3 px-4 border-b text-sm font-semibold">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(req, index) in filteredRequests" :key="req.id" class="hover:bg-gray-50">
                        <td class="py-3 px-4 border-b text-sm">{{ index + 1 }}</td>
                        <td class="py-3 px-4 border-b text-sm">{{ req?.request?.user?.userName }}</td>
                        <td class="py-3 px-4 border-b text-sm">{{ req?.request?.user?.departmentName }}</td>
                        <td class="py-3 px-4 border-b text-sm">{{ req?.request?.policyName }}</td>
                        <td class="py-3 px-4 border-b text-sm">{{ req?.request?.totalAmount }}</td>
                        <td class="py-3 px-4 border-b text-sm">{{ req?.comments }}</td>
                        <td class="py-3 px-4 border-b text-sm">{{ req?.review?.userName }}</td>
                        <td class="py-3 px-4 border-b">
                            <span :class="statusClass(req?.request?.statusName)" class="px-3 py-1 text-xs rounded-full">
                                {{ req?.request?.statusName }}
                            </span>
                        </td>
                        <td class="py-3 px-4 border-b flex gap-2 text-sm">
                            <!-- HR Department Logic -->
                            <template v-if="department == 'HR'">
                                <button v-if="!(req?.request.statusName === 'Rejected')"
                                    :disabled="req?.request?.statusName === 'Passed' || req?.request?.isApprovedByHr"
                                    :class="[
                                        'base-button',
                                        req?.statusName === 'Passed'
                                            ? 'bg-gray-300 text-gray-500 cursor-not-allowed'
                                            : 'bg-green-500 hover:bg-green-600 text-white']"
                                    class="px-3 py-1  rounded w-full" @click="openModal('accept', req?.request.id)">
                                    {{ req?.request.statusName == "Passed" ? "Accepted" : req?.request.isApprovedByHr ?
                                        "Accepted" : "Accept" }}
                                </button>
                                <button
                                    v-if="(!(department == 'HR' && req?.request?.isApprovedByHr) || (department === 'HR' && req?.request.statusName === 'Rejected'))"
                                    :disabled="req?.request.statusName === 'Passed' || req?.request.statusName === 'Rejected'"
                                    :class="[req?.statusName === 'Passed'
                                        ? 'bg-gray-300 text-gray-500 cursor-not-allowed'
                                        : 'bg-red-500 hover:bg-red-600 text-white',
                                        'base-button']" @click="openModal('reject', req?.request.id)"
                                    class="px-3 py-1 rounded">
                                    {{ req?.request.statusName === 'Rejected' && req?.request.isApprovedByHr ?
                                        "Rejected" :
                                        "Reject" }}
                                </button>
                            </template>

                            <!-- Finance Department Logic -->
                            <template v-else-if="department == 'Finance'">
                                <button
                                    :disabled="req?.request?.statusName === 'Passed' || req?.request?.isApprovedByFinance"
                                    :class="[req?.statusName === 'Passed'
                                        ? 'bg-gray-300 text-gray-500 cursor-not-allowed'
                                        : 'bg-green-500 hover:bg-green-600 text-white',
                                        'base-button']" class="px-3 py-1 rounded"
                                    @click="openModal('accept', req?.request.id)">
                                    {{ req?.request.statusName == "Passed" ? "Accepted" :
                                        req?.request.isApprovedByFinance ? "Accepted" : "Accept" }}
                                </button>
                                <button v-if="!(department == 'Finance' && req?.request?.isApprovedByFinance)"
                                    :disabled="req?.request.statusName === 'Passed'" :class="req?.statusName === 'Passed'
                                        ? 'bg-gray-300 text-gray-500 cursor-not-allowed'
                                        : 'bg-red-500 hover:bg-red-600 text-white'"
                                    @click="openModal('reject', req?.request.id)" class="px-3 py-1 rounded">
                                    {{ req?.request.statusName === 'Rejected' && !req?.request.isApprovedByFinance ?
                                        "Rejected" :
                                        "Reject" }}
                                </button>
                            </template>

                            <!-- Common View Items Button -->
                            <button @click="viewItems(req?.request?.items)"
                                class="bg-blue-500 hover:bg-blue-600 text-white px-3 py-1 rounded">
                                View Items
                            </button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>


        <div v-if="department != 'HR' && department != 'Finance'" class="overflow-x-auto">
            <div class="mt-4 flex justify-between items-center">
                <input v-model="searchQuery" @input="filterRequestManager" placeholder="Search requests..."
                    class="border border-gray-300 rounded-lg p-2 w-1/2 focus:outline-none focus:ring-2 focus:ring-blue-500" />
                <div class="flex gap-4">

                    <button @click="toggleStatusFilter('Pending')" :class="getStatusButtonClass('Pending')">
                        Pending
                    </button>
                    <button @click="toggleStatusFilter('Passed')" :class="getStatusButtonClass('Passed')">
                        Passed
                    </button>
                    <button @click="toggleStatusFilter('Rejected')" :class="getStatusButtonClass('Rejected')">
                        Rejected
                    </button>

                </div>

            </div>
            <table class="min-w-full border border-gray-300 text-left">
                <thead>
                    <tr class="bg-gray-100">
                        <th class="py-2 px-4 border-b">#</th>
                        <th class="py-2 px-4 border-b">Employee Name</th>
                        <th class="py-2 px-4 border-b">Department</th>
                        <th class="py-2 px-4 border-b">Policy</th>
                        <th class="py-2 px-4 border-b">Total Amount</th>
                        <th class="py-2 px-4 border-b">Status</th>
                        <th class="py-2 px-4 border-b">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(req, index) in filteredRequests" :key="req.id" class="hover:bg-gray-50 h-6">
                        <td class="py-2 px-4 border-b">{{ index + 1 }}</td>
                        <td class="py-2 px-4 border-b">{{ req?.user?.userName }}</td>
                        <td class="py-2 px-4 border-b">{{ req?.user?.departmentName }}</td>
                        <td class="py-2 px-4 border-b">{{ req?.policyName }}</td>
                        <td class="py-2 px-4 border-b">{{ req?.totalAmount }}</td>
                        <td class="py-2 px-4 border-b">
                            <span :class="statusClass(req?.statusName)"
                                class="px-2 py-1 rounded text-sm block w-24 text-center">
                                {{ req?.statusName }}
                            </span>
                        </td>


                        <!-- :disabled="req?.statusName === 'Passed' || req?.statusName === 'Rejected ' || (req?.statusName === 'Pending' && req?.isApprovedByManager)" -->

                        <td class="py-2 px-4 border-b flex gap-2 justify-between">
                            <button v-if="!(!req?.isApprovedByManager && req?.statusName === 'Rejected')" :class="{
                                'bg-green-400 w-full  text-white font-semibold cursor-not-allowed': req?.statusName === 'Passed' || req?.statusName === 'Rejected' || req?.isApprovedByManager,

                                'bg-green-400 hover:bg-green-600 text-white': (req?.statusName === 'Pending' || req?.statusName === 'Passed') && req?.isApprovedByManager,

                                'bg-green-600 hover:bg-green-900 font-semibold text-white': req?.statusName === 'Pending' && !req?.isApprovedByManager,

                                // 'bg-green-400 hover:bg-green-600 text-white': req?.statusName === 'Rejected'


                            }" class="px-3 py-1 rounded " @click="openModal('accept', req?.id)">
                                <span :class="{ 'text-white-600 font-semibold ': req?.isApprovedByManager }">
                                    {{ req?.isApprovedByManager ? "Accepted" : 'Accept' }}
                                </span>
                            </button>

                            <!-- :disabled="req?.statusName === 'Pending'" -->
                            <button
                                v-if="!(((req?.statusName === 'Rejected' && req?.isApprovedByManager)) || (req?.statusName === 'Pending' && req?.isApprovedByManager) || (req?.statusName === 'Passed' && req?.isApprovedByManager))"
                                :class="{

                                    'bg-red-300 hover:bg-red-600 text-white': req?.statusName === 'Passed' && req?.isApprovedByManager,

                                    'bg-red-300 hover:bg-red-600 text-white': req?.statusName === 'Rejected' && !req?.isApprovedByManager,

                                    'bg-red-500 hover:bg-red-600 text-white font-semibold': req?.statusName === 'Pending' && !req?.isApprovedByManager

                                }" class="px-3 py-1 rounded w-full " @click=" ('reject', req?.id)">
                                <span :class="{ ' font-semibold': req?.statusName === 'Rejected' }">
                                    {{ req?.statusName === 'Rejected' && !req?.isApprovedByManager ? "Rejected" :
                                        "Reject" }}
                                </span>
                            </button>

                            <button @click="viewItems(req?.items)"
                                class="bg-blue-500 hover:bg-blue-600 text-white px-3 py-1 rounded">
                                View Items
                            </button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>

        <!-- Pagination -->
        <div v-if="department != 'HR' && department != 'Finance'"
            class="mt-6 flex justify-center items-center space-x-4">
            <button @click="pageNumber--" :disabled="pageNumber === 1"
                class="px-4 py-2 bg-gray-200 hover:bg-gray-300 text-gray-700 rounded disabled:opacity-50">
                Previous
            </button>
            <span>Page {{ pageNumber }} of {{ totalPages }}</span>
            <button @click="pageNumber++" :disabled="pageNumber === totalPages"
                class="px-4 py-2 bg-gray-200 hover:bg-gray-300 text-gray-700 rounded disabled:opacity-50">
                Next
            </button>
        </div>

        <!-- Modal for Request Items -->
        <div v-if="showModal" class="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center z-50">
            <div class="bg-white p-6 rounded shadow-lg max-w-lg w-full overflow-y-auto"
                style="max-height: 90vh; width: 90vw;">
                <!-- Add these styles -->
                <h3 class="text-xl font-bold mb-4">Request Items</h3>
                <ul class="space-y-4">
                    <li v-for="item in modalItems" :key="item.id" class="border-b pb-2">
                        <p><strong>Category:</strong> {{ item.categoryName }}</p>
                        <p><strong>Description:</strong> {{ item.description }}</p>
                        <p><strong>Amount:</strong> {{ item.amount }}</p>
                        <img :src="'http://localhost:5286/File/' + item.receiptFile" alt="Receipt"
                            class="w-full h-auto mt-2 border rounded" />
                    </li>
                </ul>
                <button @click="closeModal" cl0ass="mt-4 px-4 py-2 bg-gray-700 text-white rounded hover:bg-gray-800">
                    Close
                </button>
            </div>
        </div>

        <div v-if="showCommentModel" class="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center z-50">
            <div class="bg-white p-6 rounded shadow-lg max-w-md w-full">
                <h3 class="text-xl font-bold mb-4">
                    {{ modalAction === 'accept' ? 'Approve Request' : 'Reject Request' }}
                </h3>
                <textarea v-model="comment" class="w-full p-3 border rounded" rows="4"
                    placeholder="Add your comment"></textarea>
                <div class="flex justify-end gap-2 mt-4">
                    <button @click="closeCommentModal"
                        class="bg-gray-500 hover:bg-gray-600 text-white px-4 py-2 rounded">
                        Cancel
                    </button>
                    <button @click="handleModalAction"
                        class="bg-blue-500 hover:bg-blue-600 text-white px-4 py-2 rounded">
                        Submit
                    </button>
                </div>
            </div>


        </div>
        <div v-if="showPaymentModel" class="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center z-50">
            <div class="bg-white p-6 rounded shadow-lg max-w-md w-full">
                <div v-if="bank && bank.id" class="bg-white shadow-md rounded-lg p-6">
                    <h2 class="text-2xl font-semibold mb-4">Bank Payment</h2>
                    <div class="space-y-2">
                        <p><span class="font-bold">Account Number:</span> {{ bank.accNo }}</p>
                        <p><span class="font-bold">Branch Name:</span> {{ bank.branchName }}</p>
                        <p><span class="font-bold">IFSC Code:</span> {{ bank.ifscCode }}</p>
                        <p><span class="font-bold">Branch Address:</span> {{ bank.branchAddress }}</p>
                    </div>

                    <!-- Payment Method -->
                    <div class="mt-6">
                        <label for="paymentMethod" class="font-bold mb-2 block">Payment Method:</label>
                        <select id="paymentMethod" v-model="selectedPaymentMethod"
                            class="border border-gray-300 rounded-lg w-full px-4 py-2">
                            <option disabled value="">Select Payment Method</option>
                            <option value="0">Direct Deposit</option>
                        </select>
                    </div>

                    <!-- Submit Button -->
                    <div class="mt-6">
                        <button :disabled="!selectedPaymentMethod" @click="handlePayment"
                            class="bg-blue-700 text-white px-4 py-2 rounded-lg w-full hover:bg-blue-800">
                            Pay Now
                        </button>
                    </div>

                    <!-- Success Message -->
                    <div v-if="paymentSuccess" class="mt-4 text-green-600">
                        Payment successful! The transaction has been completed.
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>


<script>
import { approveRequest, getRequestForFinance, getRequestForHR, rejectRequest } from '@/scripts/Approvals';
import { getBankByUser } from '@/scripts/Bank';
import { getPaymentByRequestId, postPayment } from '@/scripts/Payment';
import { getRequestById, getRequestbyManagerId } from '@/scripts/Request';
import { jwtDecode } from 'jwt-decode';
import { useToast } from 'vue-toastification';
export default {
    name: "EmployeeRequest",
    data() {
        return {
            request: [],
            filteredRequests: [],
            pageNumber: 1,
            pageSize: 10,
            totalPages: 1,
            showModal: false,
            showCommentModel: false,
            showPaymentModel: false,
            modalAction: null,
            modalRequestId: null,
            comment: "",
            selectedPaymentMethod: '',
            modalItems: [],
            department: "",
            bank: {},
            paymentId: {},
            paymentSuccess: false,
            toast: useToast(),
            statusFilter: [],
            searchQuery: "",
            username: ""
        }
    },
    methods: {
        toggleStatusFilter(status) {
            if (this.statusFilter.includes(status)) {
                this.statusFilter = this.statusFilter.filter((s) => s !== status);
            } else {
                this.statusFilter.push(status);
            }
            if (this.department !== "HR" && this.department !== 'Finance') {
                this.filterRequestManager()
            } else
                this.filterRequests();
        },
        filterRequests() {
            const query = this.searchQuery.toLowerCase();
            this.filteredRequests = this.request.filter((req) => {
                const matchesSearch = req?.request?.user.userName.toLowerCase().includes(query) ||
                    req?.comments.toLowerCase().includes(query) ||
                    req?.request?.statusName.toLowerCase().includes(query) ||
                    req?.request?.user?.userName.toLowerCase().includes(query);

                const matchesStatus =
                    this.statusFilter.length === 0 || this.statusFilter.includes(req.request.statusName);

                return matchesSearch && matchesStatus;
            });
        },
        filterRequestManager() {
            const query = this.searchQuery.toLowerCase();
            this.filteredRequests = this.request.filter((req) => {
                const matchesSearch = req?.user.userName.toLowerCase().includes(query) ||
                    req?.comments.toLowerCase().includes(query) ||
                    req?.statusName.toLowerCase().includes(query) ||
                    req?.policyName.toLowerCase().includes(query) ||
                    String(req?.totalAmount).toLowerCase().includes(query);

                const matchesStatus =
                    this.statusFilter.length === 0 || this.statusFilter.includes(req.statusName);

                return matchesSearch && matchesStatus;
            });

        },
        getStatusButtonClass(status) {
            const baseClass =
                "px-4 py-2 font-medium rounded transition";
            const activeClass =
                "bg-opacity-100 text-white underline decoration-double";
            const inactiveClass =
                "bg-opacity-50 text-gray-700 hover:bg-opacity-75";

            const colorClass = {
                Pending: "bg-yellow-500",
                Passed: "bg-green-500",
                Rejected: "bg-red-500",
            }[status];

            return `${baseClass} ${colorClass} ${this.statusFilter.includes(status) ? activeClass : inactiveClass
                }`;
        },
        async GetRequestManager() {
            try {
                const id = jwtDecode(sessionStorage.getItem('token'))["Id"];
                const result = await getRequestbyManagerId(+id, this.pageNumber, this.pageSize);

                console.log(result.data.data);

                this.request = result.data.data;
                this.totalPages = result.totalPages;

                this.request.sort((a, b) => {
                    const getPriority = (req) => {
                        if (!req.isApprovedByManager && (req.status === null || req.status === undefined)) {
                            return 0;
                        }
                        if (req.isApprovedByManager) {
                            return 2;
                        }
                        if (req.status === 2) {
                            return 1;
                        }
                        return 3;
                    };

                    const priorityA = getPriority(a);
                    const priorityB = getPriority(b);

                    return priorityB - priorityA;
                });
                this.filteredRequests = this.request

            } catch (err) {
                console.log(err);
            }
        },
        async GetRequestHr() {
            try {
                const result = await getRequestForHR(this.pageNumber, this.pageSize)
                console.log(result.data?.data);

                this.request = result?.data?.data
                this.request.sort((a, b) => {
                    const getPriority = (req) => {
                        if (!req.request.isApprovedByHr) {
                            return 0; // Pending
                        }
                        if (req.request.isApprovedByHr) {
                            return 1; // Approved
                        }
                        if (req.request.status === 3) {
                            return 2; // Rejected
                        }
                        return 3; // Catch-all for unexpected statuses
                    };

                    const priorityA = getPriority(a);
                    const priorityB = getPriority(b);

                    return priorityA - priorityB;
                })
                this.filteredRequests = this.request
                console.log(this.request)
            } catch (err) {
                console.log(err)
            }
        },
        async getBankUser(id) {
            try {
                const resRequest = (await getRequestById(id))
                console.log(resRequest)
                const userId = resRequest.data.data.userId
                const res = await getBankByUser(userId)
                this.bank = res.data.data
                console.log(res);

            } catch (err) {
                console.log(err)
            }

        },
        async GetRequestFinance() {
            try {

                const result = await getRequestForFinance(this.pageNumber, this.pageSize)
                this.request = result.data.data
                this.request.sort((a, b) => {
                    const getPriority = (req) => {
                        if (!req.request.isApprovedByFinance) {
                            return 0; // Pending
                        }
                        if (req.request.isApprovedByFinance) {
                            return 1; // Approved
                        }
                        if (req.request.status === 3) {
                            return 2; // Rejected
                        }
                        return 3; // Catch-all for unexpected statuses
                    };

                    const priorityA = getPriority(a);
                    const priorityB = getPriority(b);

                    return priorityA - priorityB;
                })
                this.filteredRequests = this.request
                console.log(this.filteredRequests);


            } catch (err) {
                console.log(err)
            }
        },
        async acceptRequest(id, comment) {
            const toastId = this.toast("loading..")
            try {
                const managerId = jwtDecode(sessionStorage.getItem('token'))["Id"];
                console.log(id, managerId, comment);

                const result = await approveRequest(+id, +managerId, comment);
                console.log(result);
                this.toast.update(toastId, {
                    content: "Approved succesfully",
                    options: {
                        type: 'success'
                    }
                })
                // alert("Request approved successfully.");
                this.closeCommentModal();
                if (this.department === 'Finance') {
                    this.showPaymentModel = true;
                    this.getBankUser(id)
                } else {
                    window.location.reload();
                }
            } catch (err) {
                console.error(err);
                this.toast.update(toastId, {
                    content: err.response.data.errorMessage,
                    options: {
                        type: 'error'
                    }
                })
            }
        },
        async rejectRequestHandler(id, comment) {
            try {
                const managerId = jwtDecode(sessionStorage.getItem('token'))["Id"];
                console.log(id, comment, managerId)
                const result = await rejectRequest(+id, +managerId, comment);
                console.log(result);
                // alert("Request rejected successfully.");
                this.toast.success("Request rejected successfully")
                this.closeCommentModal();
                this.
                    window.location.reload();
            } catch (err) {
                console.error(err);
            }
        },
        openModal(action, requestId) {
            this.modalAction = action;
            this.modalRequestId = requestId;
            this.showCommentModel = true;
            console.log(this.showCommentModel);
            this.comment = ""; // Reset comment input
        },
        closeCommentModal() {
            this.showCommentModel = false;
            this.modalAction = null;
            // this.modalRequestId = null;
            this.comment = "";
        },
        handleModalAction() {
            if (this.modalAction === 'accept') {
                this.acceptRequest(this.modalRequestId, this.comment);
            } else if (this.modalAction === 'reject') {
                this.rejectRequestHandler(this.modalRequestId, this.comment);
            }
        },
        statusClass(status) {
            return {
                Pending: 'bg-yellow-200 text-yellow-800',
                Passed: 'bg-green-200 text-green-800',
                Rejected: 'bg-red-200 text-red-800'
            }[status] || 'bg-gray-200 text-gray-800';
        },
        async handlePayment() {
            const toastId = this.toast('loading ...')
            try {
                const data = await getPaymentByRequestId(this.modalRequestId)
                const id = data.data.data.id;
                console.log(id, this.modalRequestId, this.selectedPaymentMethod);

                const response = await postPayment(id, this.selectedPaymentMethod);
                console.log(response)
                this.toast.update(toastId, {
                    content: "Payment successfull",
                    options: {
                        type: 'success'
                    }
                })
                this.showPaymentModel = false
            } catch (err) {
                this.toast.update(toastId, {
                    content: err.response.data.errorMessage,
                    options: {
                        type: "error"
                    }
                })
            }
        },

        viewItems(items) {
            console.log(items);

            this.modalItems = items;
            this.showModal = true;
        },
        closeModal() {
            this.showModal = false;
        }
    },
    async mounted() {
        const token = jwtDecode(sessionStorage.getItem('token'))
        console.log(token["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]);
        this.department = (token["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"])
        console.log(this.department);
        this.username = token.given_name;


        if (this.department == "HR") {

            console.log("for hr");

            await this.GetRequestHr();
        }
        else if (this.department == "Finance")
            await this.GetRequestFinance()
        else {
            await this.GetRequestManager()

        }

    }
}
</script>


<style scoped>
.base-button {
    display: inline-block;
    /* width: 120px; */
    width: 100%;
    /* Set consistent width */
    padding: 8px 12px;
    /* Adjust padding for a uniform look */
    text-align: center;
    border-radius: 4px;
    font-size: 14px;
    font-weight: 600;
    transition: all 0.3s ease;
}
</style>
