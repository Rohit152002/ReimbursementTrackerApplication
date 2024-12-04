<template>
    <div class="px-4 py-8">
        <div class="flex w-full justify-between">

            <h1 class="text-3xl font-extrabold text-blue-600 pb-20">Hi!!! {{ name }}</h1>
            <p v-if="Department" class="text-3xl font-extrabold text-blue-600 pb-20">{{ Department }}</p>
        </div>
        <div class="stats">
            <div class="stat-card" v-for="(value, key) in stats" :key="key">
                <h3>{{ key }}</h3>
                <p>{{ value }}</p>
            </div>
        </div>

        <div class="recent-activities">
            <h2>Recent Activities</h2>
            <table>
                <thead>
                    <tr>
                        <th>#</th>
                        <th>Employee Name</th>
                        <th>Status</th>
                        <th>Amount</th>
                        <th>Date</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(activity, index) in recentActivities" :key="activity.id">
                        <td>{{ index + 1 }}</td>
                        <td>{{ activity.employeeName }}</td>
                        <td>{{ activity.status }}</td>
                        <td>{{ activity.amount }}</td>
                        <td>{{ formatDate(activity.date) }}</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>

<script>
import { getDashboard } from '@/scripts/Request';
import { jwtDecode } from 'jwt-decode';

export default {
    name: "DashboardData",
    data() {
        return {
            data: {},
            name: "",
            Department: ""
        }
    }
    ,
    computed: {
        stats() {
            // Extract stats from data
            return {
                "Total Requests": this.data.totalRequests ?? 0,
                "Pending Requests": this.data.pendingRequests ?? 0,
                "Approved Requests": this.data.approvedRequests ?? 0,
                "Rejected Requests": this.data.rejectedRequests ?? 0,
                "Employees Count": this.data.employeesCount ?? 0,
            };
        },
        recentActivities() {
            return this.data.recentActivities;
        },
    },
    methods: {
        formatDate(date) {
            const options = { year: "numeric", month: "short", day: "numeric", hour: "2-digit", minute: "2-digit" };
            return new Date(date).toLocaleString(undefined, options);
        },
    },
    async mounted() {
        try {

            const res = await getDashboard()
            console.log(res.data)
            this.data = res.data;
            this.name = sessionStorage.getItem('name')


            const token = sessionStorage.getItem("token")
            const decoded = jwtDecode(token);
            this.Department = decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]



        } catch (err) {
            console.log(err)
        }
    }
};
</script>
<style>
.dashboard {
    padding: 20px;
    width: 100%;
    border: 1px solid black;
}

.stats {
    display: flex;
    gap: 20px;
    margin-bottom: 20px;
    flex-wrap: wrap;
}

.stat-card {
    background-color: #f9f9f9;
    border: 1px solid #ddd;
    border-radius: 8px;
    padding: 15px;
    text-align: center;
    flex: 1;
    min-width: 150px;
}

.stat-card h3 {
    margin: 0;
    font-size: 16px;
    color: #555;
}

.stat-card p {
    margin: 5px 0 0;
    font-size: 24px;
    font-weight: bold;
    color: #007bff;
}

.recent-activities {
    margin-top: 20px;
}

.recent-activities h2 {
    margin-bottom: 10px;
}

table {
    width: 100%;
    border-collapse: collapse;
}

table th,
table td {
    border: 1px solid #ddd;
    padding: 8px;
    text-align: left;
}

table th {
    background-color: #f4f4f4;
    font-weight: bold;
}

table tr:nth-child(even) {
    background-color: #f9f9f9;
}

table tr:hover {
    background-color: #f1f1f1;
}
</style>
