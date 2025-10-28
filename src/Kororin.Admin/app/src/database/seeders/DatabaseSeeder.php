<?php

namespace Database\Seeders;

use App\Models\Have;
use App\Models\StatusEnhancement;
use App\Models\User;

// use Illuminate\Database\Console\Seeds\WithoutModelEvents;
use Illuminate\Database\Seeder;

class DatabaseSeeder extends Seeder
{
    /**
     * Seed the application's database.
     */
    public function run(): void
    {
        // seederに呼び出し
        $this->call(UsersTableSeeder::class);
        $this->call(StagesTableSeeder::class);
        $this->call(RoomTableSeeder::class);
    }
}
