<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration {
    public function up(): void
    {
        Schema::create('rankings', function (Blueprint $table) {
            $table->id();
            $table->integer('user_id');
            $table->integer('stage_id')->index();
            $table->float('clear_time');
            $table->timestamps();
        });
    }

    public function down(): void
    {
        Schema::dropIfExists('rankings');
    }
};
